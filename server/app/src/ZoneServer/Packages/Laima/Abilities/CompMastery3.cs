using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Companion Mastery: Evasion ability.
	/// Reduces damage received from pad (ground effect) attacks by 70%.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.CompMastery3)]
	public class CompMastery3Override : IAbilityHandler
	{
		[CombatCalcModifier(CombatCalcPhase.AfterCalc_CompanionDefense, AbilityId.CompMastery3)]
		public static void OnAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (skill?.Data == null)
				return;

			if (skill.Data.HitType != SkillHitType.Pad)
				return;

			skillHitResult.Damage *= 0.30f;
		}
	}
}
