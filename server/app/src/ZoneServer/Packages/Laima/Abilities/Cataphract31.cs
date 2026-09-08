using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Two-handed Spear Mastery: Penetration ability.
	/// Increases block penetration by 5% per ability level while using a two-handed spear.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.Cataphract31)]
	public class Cataphract31Override : IAbilityHandler
	{
		/// <summary>
		/// Increases block penetration when attacking with a two-handed spear equipped.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, AbilityId.Cataphract31)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (attacker is not Character character)
				return;

			if (!attacker.TryGetActiveAbility(AbilityId.Cataphract31, out var ability))
				return;

			var weapon = character.Inventory.GetEquip(EquipSlot.RightHand);
			if (weapon == null || weapon.Data.EquipType1 != EquipType.THSpear)
				return;

			modifier.BlockPenetrationMultiplier += ability.Level * 0.05f;
		}
	}
}
