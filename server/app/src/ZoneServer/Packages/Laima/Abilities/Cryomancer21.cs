using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using System;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Shield Mastery: Cryomancer ability.
	/// Increases magic defense by 5% per ability level while using a shield.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.Cryomancer21)]
	public class Cryomancer21Override : IAbilityHandler
	{
		/// <summary>
		/// Reduces physical damage taken if wearing full plate armor.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, AbilityId.Cryomancer21)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (target is not Character character)
				return;

			if (skill.Data.AttackType != SkillAttackType.Magic)
				return;

			var lhItem = character.Inventory.GetEquip(EquipSlot.LeftHand);
			if (lhItem == null || lhItem.Data.EquipType1 != EquipType.Shield)
				return;

			if (!target.TryGetActiveAbility(AbilityId.Cryomancer21, out var ability))
				return;

			var bonus = Math.Min(ability.Level * 0.05f, 0.5f);
			skillHitResult.Damage *= 1 - bonus;
		}
	}
}
