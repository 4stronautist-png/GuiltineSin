using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Hoplite
{
	/// <summary>
	/// Handler for the passive Hoplite skill Sharp Spear, which increases
	/// the damage of critical hits with Hoplite attack skills.
	/// </summary>
	[SkillHandler(SkillId.Hoplite_SharpSpear)]
	public class Hoplite_SharpSpear : ISkillHandler
	{
		/// <summary>
		/// Applies the skill's effect during combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="attackerSkill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.AfterBonuses, SkillId.Hoplite_SharpSpear)]
		public void OnAfterBonuses(ICombatEntity attacker, ICombatEntity target, Skill attackerSkill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetSkill(SkillId.Hoplite_SharpSpear, out var skill))
				return;

			var isCrit = skillHitResult.Result == HitResultType.Crit;
			var isHopliteAttackSkill = attackerSkill.Id >= SkillId.Hoplite_Stabbing && attackerSkill.Id <= SkillId.Hoplite_SharpSpear;

			if (isCrit && isHopliteAttackSkill)
			{
				var multiplierBonus = 0.1f + skill.Level * 0.02f;
				modifier.FinalDamageMultiplier += multiplierBonus;
			}
		}
	}
}
