using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsman.Highlander
{
	/// <summary>
	/// Handle for the Cross Guard Buff, which increases the target's block rate,
	/// but prevents evasion.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.CrossGuard_Buff)]
	public class CrossGuard_BuffOverride : BuffHandler
	{
		private const float PercentageEvasionBlkBonusPerLevel = 0.05f;
		private const float BlkBonus = 200f;
		private const float BlkBonusPerLevel = 50f;
		private const float DebuffDuration = 5f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var evasion = buff.Target.Properties.GetFloat(PropertyName.DR);

			var baseValue = BlkBonus;
			var byLevel = BlkBonusPerLevel * buff.NumArg1;
			var byEvasion = PercentageEvasionBlkBonusPerLevel * buff.NumArg1 * evasion;

			var value = baseValue + byLevel + byEvasion;

			if (buff.Caster is ICombatEntity casterEntity && casterEntity.TryGetSkill(buff.SkillId, out var skill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				value *= 1f + SCR_Get_AbilityReinforceRate(skill);
			}
			value += 1f;

			AddPropertyModifier(buff, buff.Target, PropertyName.BLK_BM, value);
			AddPropertyModifier(buff, buff.Target, PropertyName.DR_BM, -evasion);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.BLK_BM);
			RemovePropertyModifier(buff, buff.Target, PropertyName.DR_BM);
		}

		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.CrossGuard_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.CrossGuard_Buff, out var buff))
				return;

			target.StartBuff(BuffId.CrossGuard_Damage_Buff, buff.NumArg1, 0, TimeSpan.FromSeconds(DebuffDuration), target);
		}
	}
}
