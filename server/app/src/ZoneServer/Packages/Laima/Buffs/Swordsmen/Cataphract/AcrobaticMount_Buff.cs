using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Cataphract
{
	/// <summary>
	/// Handler for the Acrobatic Mount buff.
	/// Grants damage and block penetration bonuses after mounted dash.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.AcrobaticMount_Buff)]
	public class AcrobaticMount_BuffOverride : BuffHandler
	{
		private const float BaseFinalDamageRate = 0.10f;
		private const float FinalDamageRatePerLevel = 0.01f;
		private const float BaseBlockPenRate = 0.10f;
		private const float BlockPenRatePerLevel = 0.01f;
		private const float BaseBlockPenFixed = 10f;
		private const float BlockPenFixedPerLevel = 4f;
		private const float MovementSpeedBonus = 5f;

		/// <summary>
		/// Applies property modifiers when buff activates.
		/// </summary>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var skillLevel = buff.NumArg1;

			var blockPenFixed = BaseBlockPenFixed + BlockPenFixedPerLevel * skillLevel;
			var blockPenRate = BaseBlockPenRate + BlockPenRatePerLevel * skillLevel;

			AddPropertyModifier(buff, target, PropertyName.BLK_BREAK_BM, blockPenFixed);
			AddPropertyModifier(buff, target, PropertyName.BLK_BREAK_RATE_BM, blockPenRate);
			AddPropertyModifier(buff, target, PropertyName.MSPD_BM, MovementSpeedBonus);
		}

		/// <summary>
		/// Removes property modifiers when buff ends.
		/// </summary>
		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			RemovePropertyModifier(buff, target, PropertyName.BLK_BREAK_BM);
			RemovePropertyModifier(buff, target, PropertyName.BLK_BREAK_RATE_BM);
			RemovePropertyModifier(buff, target, PropertyName.MSPD_BM);
		}

		/// <summary>
		/// Applies final damage bonus before attribute/race bonuses.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.AcrobaticMount_Buff)]
		public void OnAttackBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.AcrobaticMount_Buff, out var buff))
				return;

			var skillLevel = buff.NumArg1;
			var damageIncrease = BaseFinalDamageRate + FinalDamageRatePerLevel * skillLevel;

			if (buff.Caster is ICombatEntity casterEntity && casterEntity.TryGetSkill(buff.SkillId, out var buffSkill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				damageIncrease *= 1f + SCR_Get_AbilityReinforceRate(buffSkill);
			}

			skillHitResult.Damage *= 1f + damageIncrease;
		}
	}
}
