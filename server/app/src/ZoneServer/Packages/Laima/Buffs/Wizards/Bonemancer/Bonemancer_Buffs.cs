using Melia.Shared.Game.Const;
using Melia.Shared.ObjectProperties;
using Melia.Shared.Packages;
using Melia.Zone.Buffs;
using Melia.Zone.Buffs.Base;
using Melia.Zone.Scripting.ScriptableEvents;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Wizards.Bonemancer;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.Monsters;

namespace Melia.Zone.Buffs.Handlers.Wizards.Bonemancer
{
	[Package("laima")]
	[BuffHandler(BuffId.Bonemancer_Rib_Debuff)]
	public class BonemancerFractureDebuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
			=> ApplyCriticalResistanceReduction(buff, 0.01f * buff.OverbuffCounter);

		public override void OnEnd(Buff buff)
			=> RemoveCriticalResistanceReduction(buff);

		private static void ApplyCriticalResistanceReduction(Buff buff, float reductionRate)
		{
			if (buff.Target is not Mob)
			{
				UpdatePropertyModifier(buff, buff.Target, PropertyName.CRTDR_RATE_BM, -reductionRate);
				return;
			}

			var modifierVarName = ModifierVarPrefix + PropertyName.CRTDR_BM;
			buff.Vars.TryGetFloat(modifierVarName, out var previousModifier);
			var baseCriticalResistance = buff.Target.Properties.GetFloat(PropertyName.CRTDR) - previousModifier;
			UpdatePropertyModifier(buff, buff.Target, PropertyName.CRTDR_BM, -baseCriticalResistance * reductionRate);
		}

		private static void RemoveCriticalResistanceReduction(Buff buff)
			=> RemovePropertyModifier(buff, buff.Target, buff.Target is Mob ? PropertyName.CRTDR_BM : PropertyName.CRTDR_RATE_BM);
	}

	[Package("laima")]
	[BuffHandler(BuffId.Bonemancer_Backbone_Debuff)]
	public class BonemancerOssificationDebuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
			=> ApplyDefenseReduction(buff, 0.01f * buff.OverbuffCounter);

		public override void OnEnd(Buff buff)
			=> RemoveDefenseReduction(buff);

		private static void ApplyDefenseReduction(Buff buff, float reductionRate)
		{
			if (buff.Target is not Mob)
			{
				UpdatePropertyModifier(buff, buff.Target, PropertyName.DEF_RATE_BM, -reductionRate);
				return;
			}

			var modifierVarName = ModifierVarPrefix + PropertyName.DEF_BM;
			buff.Vars.TryGetFloat(modifierVarName, out var previousModifier);
			var baseDefense = buff.Target.Properties.GetFloat(PropertyName.DEF) - previousModifier;
			UpdatePropertyModifier(buff, buff.Target, PropertyName.DEF_BM, -baseDefense * reductionRate);
		}

		private static void RemoveDefenseReduction(Buff buff)
			=> RemovePropertyModifier(buff, buff.Target, buff.Target is Mob ? PropertyName.DEF_BM : PropertyName.DEF_RATE_BM);
	}

	[Package("laima")]
	[BuffHandler(BuffId.Bonemancer_RibReinforce_Debuff)]
	public class BonemancerFractureReinforcedDebuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
			=> ApplyCriticalResistanceReduction(buff, 0.15f);

		public override void OnEnd(Buff buff)
			=> RemoveCriticalResistanceReduction(buff);

		private static void ApplyCriticalResistanceReduction(Buff buff, float reductionRate)
		{
			if (buff.Target is not Mob)
			{
				UpdatePropertyModifier(buff, buff.Target, PropertyName.CRTDR_RATE_BM, -reductionRate);
				return;
			}

			var modifierVarName = ModifierVarPrefix + PropertyName.CRTDR_BM;
			buff.Vars.TryGetFloat(modifierVarName, out var previousModifier);
			var baseCriticalResistance = buff.Target.Properties.GetFloat(PropertyName.CRTDR) - previousModifier;
			UpdatePropertyModifier(buff, buff.Target, PropertyName.CRTDR_BM, -baseCriticalResistance * reductionRate);
		}

		private static void RemoveCriticalResistanceReduction(Buff buff)
			=> RemovePropertyModifier(buff, buff.Target, buff.Target is Mob ? PropertyName.CRTDR_BM : PropertyName.CRTDR_RATE_BM);
	}

	[Package("laima")]
	[BuffHandler(BuffId.Bonemancer_BackboneReinforce_Debuff)]
	public class BonemancerOssificationReinforcedDebuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
			=> ApplyDefenseReduction(buff, 0.15f);

		public override void OnEnd(Buff buff)
			=> RemoveDefenseReduction(buff);

		private static void ApplyDefenseReduction(Buff buff, float reductionRate)
		{
			if (buff.Target is not Mob)
			{
				UpdatePropertyModifier(buff, buff.Target, PropertyName.DEF_RATE_BM, -reductionRate);
				return;
			}

			var modifierVarName = ModifierVarPrefix + PropertyName.DEF_BM;
			buff.Vars.TryGetFloat(modifierVarName, out var previousModifier);
			var baseDefense = buff.Target.Properties.GetFloat(PropertyName.DEF) - previousModifier;
			UpdatePropertyModifier(buff, buff.Target, PropertyName.DEF_BM, -baseDefense * reductionRate);
		}

		private static void RemoveDefenseReduction(Buff buff)
			=> RemovePropertyModifier(buff, buff.Target, buff.Target is Mob ? PropertyName.DEF_BM : PropertyName.DEF_RATE_BM);
	}

	[Package("laima")]
	[BuffHandler(BuffId.Bonemancer_FistReinforce_Debuff)]
	public class BonemancerPunctureReinforcedDebuff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Bonemancer_FistReinforce_Debuff)]
		public void OnBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult result)
		{
			if (!BonemancerSkillHelper.IsBonemancerDamageSkill(skill.Id))
				return;

			modifier.FinalDamageMultiplier *= target is Mob mob && mob.Rank == MonsterRank.Boss ? 1.3f : 1.1f;
		}
	}

	[Package("laima")]
	[BuffHandler(BuffId.BoneShield_Buff)]
	public class BoneShieldBuff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.BoneShield_Buff)]
		public void OnBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult result)
		{
			if (target.TryGetBuff(BuffId.BoneShield_Buff, out var buff))
				modifier.FinalDamageMultiplier *= 1f - 0.03f * buff.NumArg1;
		}
	}

	[Package("laima")]
	[BuffHandler(BuffId.BoneShield_Abil_Buff)]
	public class BoneShieldUnifiedStructureBuff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.BoneShield_Abil_Buff)]
		public void OnBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult result)
		{
			if (!BonemancerSkillHelper.IsBonemancerDamageSkill(skill.Id))
				return;

			if (attacker.TryGetBuff(BuffId.BoneShield_Abil_Buff, out var buff))
				modifier.FinalDamageMultiplier *= 1f + 0.01f * buff.NumArg1;
		}
	}
}
