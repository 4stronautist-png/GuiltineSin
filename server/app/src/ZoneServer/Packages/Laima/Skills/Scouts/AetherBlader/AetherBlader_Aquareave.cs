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
	[SkillHandler(SkillId.AetherBlader_Aquareave_Wizard)]
	public class AetherBlader_AquareaveWizard : AetherBlader_Aquareave
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Aquareave_Cleric)]
	public class AetherBlader_AquareaveCleric : AetherBlader_Aquareave
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Aquareave_Scout)]
	public class AetherBlader_AquareaveScout : AetherBlader_Aquareave
	{
	}

	public class AetherBlader_Aquareave : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const int RetailPuddleVisualSkillLevel = 6;
		private const float PuddleDistanceRatio = 0.84f;
		private const float PuddleSideOffsetRatio = 0.18f;
		private const float RetailPuddleSideArg = 0.21f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			skill.IncreaseOverheat();

			var direction = AetherBladerSkillHelper.GetCasterForwardDirection(caster);
			var targetPos = AetherBladerSkillHelper.GetForwardPosition(caster, skill.Properties.GetFloat(PropertyName.MaxR));
			var area = new Square(caster.Position, direction, skill.Properties.GetFloat(PropertyName.MaxR), skill.Properties.GetFloat(PropertyName.SplRange));
			var puddlePos = this.GetPuddlePosition(caster, skill, direction);
			var hasPuddleSlot = AetherBladerSkillHelper.ReservePuddleSlots(caster, 1) > 0;

			this.ShowBlade(skill, caster);
			AetherBladerSkillHelper.PrepareGroundSkill(skill, caster, caster.Position, targetPos, direction);
			AetherBladerSkillHelper.SendAetherBladeCastEffect(caster);
			skill.Run(this.HideBlade(skill, caster));

			if (hasPuddleSlot)
				AetherBladerSkillHelper.CreatePuddle(caster, skill, puddlePos, sendPadVisual: true, consumeReservedSlot: true, visualDirection: direction.Left, visualArg1: RetailPuddleSideArg, visualArg2: (float)caster.Position.Get2DDistance(puddlePos), visualSkillLevel: RetailPuddleVisualSkillLevel);

			var skillTargets = AetherBladerSkillHelper.GetTargets(caster, skill, area);
			var hits = AetherBladerSkillHelper.DealHits(caster, skill, skillTargets, t => AetherBladerSkillHelper.BuildModifier(skill, t, AetherBladerSkillHelper.GetTideCallFinalDamageBonus(caster)));
			AetherBladerSkillHelper.ApplyDrenched(caster, skill, hits.Select(hit => hit.Target));
		}

		private void ShowBlade(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.AetherBlader_Blade_Buff, skill.Level, 0, TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true);
		}

		private async System.Threading.Tasks.Task HideBlade(Skill skill, ICombatEntity caster)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(800));
			caster.RemoveBuff(BuffId.AetherBlader_Blade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false);
		}

		private Position GetPuddlePosition(ICombatEntity caster, Skill skill, Direction direction)
		{
			var maxRange = Math.Max(35f, skill.Properties.GetFloat(PropertyName.MaxR));
			var forwardDistance = maxRange * PuddleDistanceRatio;
			var sideOffset = maxRange * PuddleSideOffsetRatio;
			var position = caster.Position.GetRelative(direction, forwardDistance) + direction.Left * sideOffset;
			return caster.Map.Ground.GetLastValidPosition(caster.Position, position);
		}

	}
}
