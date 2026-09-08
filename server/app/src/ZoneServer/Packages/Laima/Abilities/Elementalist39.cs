using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Elementalist: Elemental Exploitation ability.
	/// Increases final damage by 0.5% per ability level for each
	/// different elemental condition (Fire, Freeze, Shock) on the target.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.Elementalist39)]
	public class Elementalist39Override : IAbilityHandler
	{
		private const float DamagePerLevelPerCondition = 0.005f;

		[CombatCalcModifier(CombatCalcPhase.AfterCalc, AbilityId.Elementalist39)]
		public void OnAttackAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetActiveAbility(AbilityId.Elementalist39, out var ability))
				return;

			var conditionCount = 0;

			if (target.IsBuffActiveByKeyword(BuffTag.Fire))
				conditionCount++;

			if (target.IsBuffActiveByKeyword(BuffTag.Freeze))
				conditionCount++;

			if (target.IsBuffActiveByKeyword(BuffTag.Shock))
				conditionCount++;

			if (conditionCount == 0)
				return;

			var bonus = 1f + DamagePerLevelPerCondition * ability.Level * conditionCount;
			skillHitResult.Damage *= bonus;
		}
	}
}
