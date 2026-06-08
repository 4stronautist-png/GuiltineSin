using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melia.Shared.Data.Database;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Network.Helpers;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Wizards.Bonemancer
{
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReinforcement_Swordman)] public class Bonemancer_BoneReinforcementSwordman : Bonemancer_BoneReinforcement { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReinforcement_Wizard)] public class Bonemancer_BoneReinforcementWizard : Bonemancer_BoneReinforcement { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReinforcement_Archer)] public class Bonemancer_BoneReinforcementArcher : Bonemancer_BoneReinforcement { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReinforcement_Cleric)] public class Bonemancer_BoneReinforcementCleric : Bonemancer_BoneReinforcement { }

	public class Bonemancer_BoneReinforcement : IDynamicCasted
	{
		private const float PadRange = 30f;
		private const float TrailPointSpacing = 30f;
		private const int MaxTargets = 5;
		private const int HitsPerTick = 2;
		private static readonly TimeSpan ChannelDuration = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan VisualDuration = TimeSpan.FromMilliseconds(1500);
		private static readonly TimeSpan PadDuration = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(500);
		private static readonly TimeSpan TrailUpdateInterval = TimeSpan.FromMilliseconds(100);
		private const int VisualPulseCount = 2;

		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			var spMultiplier = caster.IsAbilityActive(AbilityId.Bonemancer9) ? 1.5f : 1f;
			if (!BonemancerSkillHelper.TrySpendSp(caster, skill, spMultiplier))
				return;
			skill.IncreaseOverheat();

			skill.RunFree(this.Channel(skill, caster, caster.Direction));
		}

		private async Task Channel(Skill skill, ICombatEntity caster, Direction direction)
		{
			var effectHandle = ForceId.GetNew();
			var visualPosition = caster.Position;
			var startTime = DateTime.UtcNow;
			var channelEndTime = startTime + ChannelDuration;
			var visualEndTime = startTime + VisualDuration;
			var nextDamageTime = startTime + TickInterval;
			var nextPulseTime = startTime + TickInterval;
			var trailPoints = new List<TrailPoint>();
			var channelStopped = false;
			var visualStopped = false;
			var visualPulsesSent = 0;

			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.Bonemancer_BoneReinforcement, skill, visualPosition, direction, 0f, 0f, effectHandle, PadRange, true);
			caster.StartBuff(BuffId.BoneReinforcement_Buff, skill.Level, 0, ChannelDuration, caster, skill.Id);
			caster.StartBuff(BuffId.IS_Channeling_Buff, skill.Level, 0, TimeSpan.FromSeconds(5), caster, skill.Id);

			while (!channelStopped || trailPoints.Count > 0)
			{
				var now = DateTime.UtcNow;
				var channelActive = now < channelEndTime && !caster.IsDead;

				if (channelActive)
					this.RefreshTrailPoint(trailPoints, caster.Position, now + PadDuration);

				for (var i = trailPoints.Count - 1; i >= 0; i--)
				{
					if (trailPoints[i].ExpiresAt <= now)
						trailPoints.RemoveAt(i);
				}

				if (!visualStopped && visualPulsesSent < VisualPulseCount && now >= nextPulseTime)
				{
					BonemancerSkillHelper.PlayRetailBonePulse(caster);
					nextPulseTime = now + TickInterval;
					visualPulsesSent++;
				}

				if (now >= nextDamageTime)
				{
					this.DamageTrail(skill, caster, trailPoints);
					nextDamageTime = now + TickInterval;
				}

				if (!visualStopped && (now >= visualEndTime || !channelActive))
				{
					Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.Bonemancer_BoneReinforcement, skill, visualPosition, direction, 0f, 0f, effectHandle, PadRange, false);
					visualStopped = true;
				}

				if (!channelActive && !channelStopped)
				{
					caster.RemoveBuff(BuffId.BoneReinforcement_Buff);
					caster.RemoveBuff(BuffId.IS_Channeling_Buff);
					channelStopped = true;
				}

				await skill.Wait(TrailUpdateInterval);
			}
		}

		private void RefreshTrailPoint(List<TrailPoint> trailPoints, Position position, DateTime expiresAt)
		{
			foreach (var point in trailPoints)
			{
				if (point.Position.Get2DDistance(position) > TrailPointSpacing)
					continue;

				point.ExpiresAt = expiresAt;
				return;
			}

			trailPoints.Add(new TrailPoint(position, expiresAt));
		}

		private void DamageTrail(Skill skill, ICombatEntity caster, List<TrailPoint> trailPoints)
		{
			var hitTargets = new HashSet<int>();
			var maxTargets = skill.GetPVPValue(MaxTargets);

			foreach (var point in trailPoints)
			{
				var area = new Circle(point.Position, PadRange);
				var targets = BonemancerSkillHelper.GetFixedCountTargets(caster, skill, area, maxTargets);
				foreach (var target in targets)
				{
					if (!hitTargets.Add(target.Handle))
						continue;

					var modifier = SkillModifier.MultiHit(HitsPerTick);
					modifier.FinalDamageMultiplier *= 1f + 0.01f * BonemancerSkillHelper.GetPunctureStacks(target);
					BonemancerSkillHelper.DealHits(caster, skill, [target], modifier);
					BonemancerSkillHelper.ApplyReinforcedDebuffs(caster, skill, target);
				}
			}
		}

		private sealed class TrailPoint
		{
			public Position Position { get; }
			public DateTime ExpiresAt { get; set; }

			public TrailPoint(Position position, DateTime expiresAt)
			{
				this.Position = position;
				this.ExpiresAt = expiresAt;
			}
		}
	}
}
