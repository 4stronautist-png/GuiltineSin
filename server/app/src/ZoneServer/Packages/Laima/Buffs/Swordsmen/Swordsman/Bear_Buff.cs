using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Bear buff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Bear_Buff)]
	public class Bear_BuffOverride : BuffHandler
	{
		private const float BaseDamageReduction = 0.075f;
		private const float DamageReductionPerLevel = 0.015f;

		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Bear_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Bear_Buff, out var buff))
				return;

			var skillLevel = buff.NumArg1;
			var multiplierReduction = BaseDamageReduction + skillLevel * DamageReductionPerLevel;

			if (buff.Caster is ICombatEntity casterEntity && casterEntity.TryGetSkill(buff.SkillId, out var buffSkill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				multiplierReduction *= 1f + SCR_Get_AbilityReinforceRate(buffSkill);
			}

			multiplierReduction = Math.Min(0.80f, multiplierReduction);

			// We originally reduced the damage directly from inside the combat
			// calculations, on AfterBonuses, but setting the multiplier seems
			// much easier. Is this correct? Who knows.

			modifier.DamageMultiplier *= (1f - multiplierReduction);
		}
	}
}
