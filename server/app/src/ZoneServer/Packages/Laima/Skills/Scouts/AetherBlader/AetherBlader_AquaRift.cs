using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_AquaRift_Wizard)]
	public class AetherBlader_AquaRiftWizard : AetherBlader_AquaRift
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_AquaRift_Cleric)]
	public class AetherBlader_AquaRiftCleric : AetherBlader_AquaRift
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_AquaRift_Scout)]
	public class AetherBlader_AquaRiftScout : AetherBlader_AquaRift
	{
	}

	public class AetherBlader_AquaRift : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const int RetailVisualSkillLevel = 16;
		private const float FirstPillarDistance = 50f;
		private const float PillarSpacing = 70f;
		private const int PillarCount = 3;
		private static readonly TimeSpan PillarInterval = TimeSpan.FromMilliseconds(200);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			skill.IncreaseOverheat();

			var direction = AetherBladerSkillHelper.GetCasterForwardDirection(caster);
			var pillarPositions = this.GetPillarPositions(caster, direction);
			var puddleCount = AetherBladerSkillHelper.ReservePuddleSlots(caster, pillarPositions.Count);
			var targetPos = pillarPositions[0];

			this.ShowBlade(skill, caster);
			AetherBladerSkillHelper.PrepareGroundSkill(skill, caster, caster.Position, targetPos, direction);
			AetherBladerSkillHelper.SendAetherBladeCastEffect(caster);

			skill.Run(this.Attack(skill, caster, pillarPositions, direction, puddleCount));
			skill.Run(this.HideBlade(skill, caster));
		}

		private void ShowBlade(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.AetherBlader_Blade_Buff, skill.Level, 0, TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true);
		}

		private async System.Threading.Tasks.Task HideBlade(Skill skill, ICombatEntity caster)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(1300));
			caster.RemoveBuff(BuffId.AetherBlader_Blade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false);
		}

		private List<Position> GetPillarPositions(ICombatEntity caster, Direction direction)
		{
			var positions = new List<Position>();
			for (var i = 0; i < PillarCount; ++i)
			{
				var distance = FirstPillarDistance + PillarSpacing * i;
				var position = caster.Position.GetRelative(direction, distance);
				positions.Add(caster.Map.Ground.GetLastValidPosition(caster.Position, position));
			}

			return positions;
		}

		private void ShowPillar(Skill skill, ICombatEntity caster, Position position, Direction direction, float distance)
		{
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.AetherBlader_AquaRift, skill, position, direction, 0f, distance, ForceId.GetNew(), 40f, true, visualSkillLevel: RetailVisualSkillLevel);
		}

		private async System.Threading.Tasks.Task Attack(Skill skill, ICombatEntity caster, IReadOnlyList<Position> pillarPositions, Direction direction, int puddleCount)
		{
			var firstPillarDelay = skill.Data.HitTime.Count > 0 ? skill.Data.HitTime[0] : PillarInterval;
			var hitTargets = new HashSet<ICombatEntity>();

			for (var i = 0; i < pillarPositions.Count; ++i)
			{
				await skill.Wait(i == 0 ? firstPillarDelay : PillarInterval);

				var distance = FirstPillarDistance + PillarSpacing * i;
				var pillarPosition = pillarPositions[i];

				this.ShowPillar(skill, caster, pillarPosition, direction, distance);
				AetherBladerSkillHelper.SendAetherGroundImpactEffect(caster, pillarPosition);

				var area = new Circle(pillarPosition, skill.Properties.GetFloat(PropertyName.SplRange));
				var skillTargets = AetherBladerSkillHelper.GetTargets(caster, skill, area)
					.Where(target => hitTargets.Add(target))
					.ToList();
				var hits = AetherBladerSkillHelper.DealHits(caster, skill, skillTargets, t => AetherBladerSkillHelper.BuildModifier(skill, t, AetherBladerSkillHelper.GetTideCallFinalDamageBonus(caster)));
				AetherBladerSkillHelper.ApplyDrenched(caster, skill, hits.Select(hit => hit.Target));
			}

			var createdPuddles = 0;
			try
			{
				await skill.Wait(skill.Data.HoldTime.Count > 0 ? skill.Data.HoldTime[0] : TimeSpan.FromSeconds(1));
				for (var i = 0; i < puddleCount; ++i)
				{
					var distance = FirstPillarDistance + PillarSpacing * i;
					if (AetherBladerSkillHelper.CreatePuddle(caster, skill, pillarPositions[i], AetherBladerSkillHelper.AquaRiftPuddleGroundEffectKey, sendPadVisual: true, consumeReservedSlot: true, visualDirection: direction.Left, visualArg2: distance, visualSkillLevel: RetailVisualSkillLevel) != null)
						createdPuddles++;
				}
			}
			finally
			{
				AetherBladerSkillHelper.ReleasePuddleSlots(caster, puddleCount - createdPuddles);
			}
		}
	}
}
