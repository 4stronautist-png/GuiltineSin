using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_ArcaneCollapse_Wizard)]
	public class AetherBlader_ArcaneCollapseWizard : AetherBlader_ArcaneCollapse
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_ArcaneCollapse_Cleric)]
	public class AetherBlader_ArcaneCollapseCleric : AetherBlader_ArcaneCollapse
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_ArcaneCollapse_Scout)]
	public class AetherBlader_ArcaneCollapseScout : AetherBlader_ArcaneCollapse
	{
	}

	public class AetherBlader_ArcaneCollapse : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const int RetailVisualSkillLevel = 11;
		private const int ElectricBladeItemId = 121136;
		private const int ElectricBladeEffectStringId = 1828;

		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.StartBuff(BuffId.AetherBlader_ElectBlade_Buff, skill.Level, 0, TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true, ElectricBladeItemId, ElectricBladeEffectStringId);
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.RemoveBuff(BuffId.AetherBlader_ElectBlade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false, ElectricBladeItemId, ElectricBladeEffectStringId);
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			skill.IncreaseOverheat();

			var direction = this.GetCasterForwardDirection(caster);
			var targetPos = this.GetFixedImpactPosition(caster, skill, direction);
			var overloadSuccess = this.RollOverloadSuccess(caster);
			this.AddOverloadStack(skill, caster);

			if (!overloadSuccess)
			{
				this.PrepareArcaneCollapseFailureBlock(skill, caster, targetPos, direction);
				return;
			}

			var effectHandle = ForceId.GetNew();
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.AetherBlader_ArcaneCollapse, skill, targetPos, direction, 0f, 0f, effectHandle, 30f, true, visualSkillLevel: RetailVisualSkillLevel);
			this.PrepareArcaneCollapseVisualBlock(skill, caster, targetPos, direction);
			skill.Run(this.HideArcaneCollapseEffect(skill, caster, targetPos, direction, effectHandle));
			skill.Run(this.PlayDelayedArcaneCollapsePulses(skill, caster, targetPos, direction));

			var area = new Circle(targetPos, AetherBladerSkillHelper.ArcaneCollapseRadius);
			var targetsInArea = AetherBladerSkillHelper.GetTargets(caster, skill, area);
			AetherBladerSkillHelper.DealHits(caster, skill, targetsInArea, t =>
			{
				var bonus = AetherBladerSkillHelper.GetTideCallFinalDamageBonus(caster);
				if (t.IsBuffActive(BuffId.Wet_Debuff) || t.IsBuffActive(BuffId.FrostSatter_Debuff))
					bonus += 0.30f;
				return AetherBladerSkillHelper.BuildModifier(skill, t, bonus);
			});
		}

		private Position GetFixedImpactPosition(ICombatEntity caster, Skill skill, Direction direction)
		{
			var distance = skill.Properties.GetFloat(PropertyName.MaxR);
			if (distance <= 0)
				distance = 100f;

			var position = caster.Position.GetRelative(direction, distance);
			return caster.Map.Ground.GetLastValidPosition(caster.Position, position);
		}

		private Direction GetCasterForwardDirection(ICombatEntity caster)
			=> AetherBladerSkillHelper.GetCasterForwardDirection(caster);

		private bool RollOverloadSuccess(ICombatEntity caster)
		{
			if (!caster.TryGetBuff(BuffId.AetherBlader_ArcaneCollapse_UseStack_Buff, out var buff))
				return true;

			var successChance = buff.OverbuffCounter <= 1 ? 40 : 20;
			return Random.Shared.Next(100) < successChance;
		}

		private void AddOverloadStack(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.AetherBlader_ArcaneCollapse_UseStack_Buff, skill.Level, 0, TimeSpan.FromSeconds(15), caster, skill.Id);
		}

		private void PrepareArcaneCollapseVisualBlock(Skill skill, ICombatEntity caster, Position targetPos, Direction direction)
		{
			var syncKey = ForceId.GetNew();

			caster.SetAttackState(true);
			Send.ZC_SKILL_READY(caster, skill, caster.Position, targetPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, caster.Position, direction, Position.Zero);
			Send.ZC_SYNC_START(caster, syncKey, 1f);
			AetherBladerSkillHelper.SendArcaneCollapseOpeningEffects(caster, targetPos);
			Send.ZC_SYNC_END(caster, syncKey, 0f);
			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, syncKey, TimeSpan.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos, ForceId.GetNew(), null);
		}

		private void PrepareArcaneCollapseFailureBlock(Skill skill, ICombatEntity caster, Position targetPos, Direction direction)
		{
			var syncKey = ForceId.GetNew();

			caster.SetAttackState(true);
			Send.ZC_SYNC_START(caster, syncKey, 1f);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, caster.Position, direction, Position.Zero);
			Send.ZC_SKILL_READY(caster, skill, caster.Position, targetPos);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos, ForceId.GetNew(), null);
			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, syncKey, TimeSpan.Zero);
			Send.ZC_SYNC_END(caster, syncKey, 0f);
			Send.ZC_PLAY_SOUND(caster, "skl_eff_aetherblader_arcanecollapse_fail");
			Send.ZC_PLAY_FULLSCREEN_EFFECT_AetherBladerArcaneCollapseFail(caster);
		}

		private async Task PlayDelayedArcaneCollapsePulses(Skill skill, ICombatEntity caster, Position position, Direction direction)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(160));
			AetherBladerSkillHelper.SendAetherGroundImpactEffect(caster, position);
			AetherBladerSkillHelper.SendArcaneCollapsePulseEffects(caster, position, unchecked((int)0x3A78FFFF), 0x3A780000, true);

			await skill.Wait(TimeSpan.FromMilliseconds(520));
			AetherBladerSkillHelper.SendArcaneCollapsePulseEffects(caster, position, unchecked((int)0xC238FFFF), unchecked((int)0xC238FFFF), true);

			await skill.Wait(TimeSpan.FromMilliseconds(520));
			AetherBladerSkillHelper.SendAetherGroundImpactEffect(caster, position);
			AetherBladerSkillHelper.SendArcaneCollapsePulseEffects(caster, position, unchecked((int)0xAF78FFFF), unchecked((int)0xAF780000), true);

			await skill.Wait(TimeSpan.FromMilliseconds(520));
			AetherBladerSkillHelper.SendArcaneCollapsePulseEffects(caster, position, 0x15F80000, 0x15F80000, true);

			await skill.Wait(TimeSpan.FromMilliseconds(520));
			AetherBladerSkillHelper.SendAetherGroundImpactEffect(caster, position);
			AetherBladerSkillHelper.SendArcaneCollapsePulseEffects(caster, position, unchecked((int)0xE1B8FFFF), unchecked((int)0xE1B80000), true);
		}

		private async Task HideArcaneCollapseEffect(Skill skill, ICombatEntity caster, Position position, Direction direction, int effectHandle)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(3560));
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.AetherBlader_ArcaneCollapse, skill, position, direction, 0f, 0f, effectHandle, 30f, false, visualSkillLevel: RetailVisualSkillLevel);
		}
	}
}
