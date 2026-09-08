using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Shared.Data.Database;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	/// <summary>
	/// Plate Mastery (Iron) ability.
	/// Reduces physical damage taken by 20% when wearing full plate armor.
	/// </summary>
	[Package("laima")]
	[AbilityHandler(AbilityId.Iron)]
	public class IronOverride : IAbilityHandler
	{
		/// <summary>
		/// Reduces physical damage taken if wearing full plate armor.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, AbilityId.Iron)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (target is not Character character)
				return;

			if (skill.Data.AttackType == SkillAttackType.Magic)
				return;

			var count = character.Inventory.CountEquipMaterial(ArmorMaterialType.Iron);
			if (count >= 4)
			{
				skillHitResult.Damage *= 0.7f;
			}
		}
	}
}
