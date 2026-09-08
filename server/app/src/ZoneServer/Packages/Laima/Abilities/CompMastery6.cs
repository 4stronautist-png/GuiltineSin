using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Companion Mastery: Thick Hide ability.
	/// Reduces physical damage received by the companion by 25%.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.CompMastery6)]
	public class CompMastery6Override : IAbilityHandler
	{
		[CombatCalcModifier(CombatCalcPhase.AfterCalc_CompanionDefense, AbilityId.CompMastery6)]
		public static void OnAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (skill?.Data == null)
				return;

			if (skill.Data.ClassType == SkillClassType.Magic)
				return;

			skillHitResult.Damage *= 0.75f;
		}
	}
}
