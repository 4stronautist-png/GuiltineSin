using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.HandlersOverrides.Swordsmen.Hoplite
{
	/// <summary>
	/// Handler override for the Hoplite passive skill Sharp Spear.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Hoplite_SharpSpear)]
	public class Hoplite_SharpSpearOverride : ISkillHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, SkillId.Hoplite_SharpSpear)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			// Check if the attacking skill is pierce type (Aries)
			var isPierceType = skill.Data.AttackType == SkillAttackType.Aries;

			if (isPierceType)
			{
				if (!attacker.TryGetSkill(SkillId.Hoplite_SharpSpear, out var sharpSpearSkill))
					return;

				// Base 20% + 2% per skill level
				var critRateBonus = 0.20f + (sharpSpearSkill.Level * 0.02f);

				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				critRateBonus *= 1f + SCR_Get_AbilityReinforceRate(sharpSpearSkill);

				modifier.CritRateMultiplier += critRateBonus;
			}
		}
	}
}
