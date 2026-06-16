using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.Components;

namespace Melia.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_TempestShot_Scout)]
	public class BlitzHunter_TempestShotOverride : BlitzHunter_TempestShotBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_TempestShot_Archer)]
	public class BlitzHunter_TempestShotArcherOverride : BlitzHunter_TempestShotBase
	{
	}

	public class BlitzHunter_TempestShotBase : IMeleeGroundSkillHandler, IGroundSkillHandler
	{
		private const string CastAnimation = "SKL_TEMPESTSHOT";
		private const int MaxTargets = 10;
		private const float EffectDistance = 40f;
		private const int RetailVisualSkillLevel = 2;
		private const string NormalCastSound = "skl_eff_blitzhunter_tempestshot";
		private const string BlitzkriegCastSound = "skl_eff_blitzhunter_tempestshot_blitzkrieg";
		private static readonly TimeSpan NormalAnimationLock = TimeSpan.FromMilliseconds(2650);
		private static readonly TimeSpan BlitzkriegAnimationLock = TimeSpan.FromMilliseconds(1425);
		private static readonly TimeSpan EffectCleanupDelay = TimeSpan.FromMilliseconds(150);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, System.Collections.Generic.IList<ICombatEntity> targets)
			=> this.HandleCore(skill, caster, originPos, farPos);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
			=> this.HandleCore(skill, caster, originPos, farPos);

		private void HandleCore(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			skill.IncreaseOverheat();
			caster.SetAttackState(true);
			// caster.Lock(LockType.Movement, isBlitzkrieg ? BlitzkriegAnimationLock : NormalAnimationLock);
			Send.ZC_PC_ATKSTATE(caster, true, includeEntity: true);
			BlitzHunterSkillHelper.TryPlayAnimation(caster, CastAnimation);

			var direction = BlitzHunterSkillHelper.GetDirection(caster, originPos, farPos);
			var effectOrigin = new Position(caster.Position.X, 0f, caster.Position.Z);
			var clampedFarPos = caster.Position.GetRelative(direction, skill.Properties.GetFloat(PropertyName.MaxR));

			BlitzHunterSkillHelper.SendGroundSkillReady(caster, skill);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, clampedFarPos, ForceId.GetNew(), null);
			skill.Run(this.Attack(skill, caster, effectOrigin, clampedFarPos, direction));
		}

		private async System.Threading.Tasks.Task Attack(Skill skill, ICombatEntity caster, Position effectOrigin, Position clampedFarPos, Direction direction)
		{
			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var effectMoments = isBlitzkrieg ? new[] { 500, 800, 1225 } : new[] { 1000, 1600, 2450 };
			var hitMoments = isBlitzkrieg ? new[] { 700, 1000, 1425 } : new[] { 1200, 1800, 2650 };
			var effectAngle = (float)(Math.PI / 2 - Math.Atan2(direction.Sin, direction.Cos));
			var elapsed = TimeSpan.Zero;
			var shouldApplyVoltic = false;

			for (var i = 0; i < effectMoments.Length; i++)
			{
				var effectDelay = TimeSpan.FromMilliseconds(effectMoments[i]);
				var beforeEffect = effectDelay - elapsed;
				if (beforeEffect > TimeSpan.Zero)
				{
					await skill.Wait(beforeEffect);
					elapsed = effectDelay;
				}

				if (i == 0)
					BlitzHunterSkillHelper.TryPlaySound(caster, isBlitzkrieg ? BlitzkriegCastSound : NormalCastSound);

				var stagedEffectHandle = BlitzHunterSkillHelper.CreateRetailVisualPadHandle();
				this.SendTempestShotPad(caster, skill, effectOrigin, clampedFarPos, direction, effectAngle, stagedEffectHandle, true);

				var hitDelay = TimeSpan.FromMilliseconds(hitMoments[i]);
				var beforeHit = hitDelay - elapsed;
				if (beforeHit > TimeSpan.Zero)
				{
					await skill.Wait(beforeHit);
					elapsed = hitDelay;
				}

				var skillTargets = BlitzHunterSkillHelper.GetForwardTargets(caster, skill, direction, skill.Properties.GetFloat(PropertyName.MaxR), 20f, MaxTargets);
				var totalMultiHitCount = skill.Properties.MultiHitCount > 0 ? skill.Properties.MultiHitCount : 9;
				var stageHitCount = Math.Max(1, totalMultiHitCount / effectMoments.Length);
				var hits = BlitzHunterSkillHelper.DealDirectHits(caster, skill, skillTargets, SkillModifier.MultiHit(stageHitCount), BlitzHunterSkillHelper.GetScaledDisplayDelay(skill, 50), hitCountOverride: stageHitCount);

				foreach (var hit in hits)
					Send.ZC_HIT_INFO(caster, hit.Target, hit);

				if (!shouldApplyVoltic && BlitzHunterSkillHelper.HasAccurateHit(hits))
					shouldApplyVoltic = true;

				await skill.Wait(EffectCleanupDelay);
				elapsed += EffectCleanupDelay;

				if (i == effectMoments.Length - 1)
				{
					Send.ZC_NORMAL.SkillCancel(caster, skill.Id);
					Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
				}

				this.SendTempestShotPad(caster, skill, effectOrigin, clampedFarPos, direction, effectAngle, stagedEffectHandle, false);
			}

			if (shouldApplyVoltic)
				BlitzHunterSkillHelper.ApplyVolticCatharsis(caster, skill);

			Send.ZC_NORMAL.ResetStdAnim(caster);
		}

		private void SendTempestShotPad(ICombatEntity caster, Skill skill, Position effectOrigin, Position effectDestination, Direction direction, float effectAngle, int effectHandle, bool isVisible)
		{
			var position = isVisible ? effectOrigin : effectDestination;
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, "BlitzHunter_TempestShot_Pad", skill, position, direction, effectAngle, 0f, effectHandle, EffectDistance, isVisible, visualSkillLevel: RetailVisualSkillLevel);

			if (isVisible)
				Send.ZC_NORMAL.SkillEffectMovement(caster, effectHandle, effectDestination, 1000f, 200f);
		}
	}
}
