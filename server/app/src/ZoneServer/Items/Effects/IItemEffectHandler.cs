using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Items;

namespace GuiltineSin.Zone.Items.Effects
{
	/// <summary>
	/// Hook type for different combat calculation phases
	/// </summary>
	public enum ItemHookType
	{
		AttackBeforeCalc,
		DefenseBeforeCalc,
		AttackBeforeBonuses,
		AttackAfterBonuses,
		DefenseBeforeBonuses,
		DefenseAfterBonuses,
		DebuffResist,
		ItemUse,
		Kill,
		Dead,
	}

	/// <summary>
	/// Delegate for item combat hooks
	/// </summary>
	public delegate void ItemCombatHook(Item item, ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult);

	/// <summary>
	/// Delegate for debuff resistance hooks.
	/// Returns the resistance rate (0.0 to 1.0) to add for this debuff.
	/// </summary>
	public delegate float ItemDebuffResistHook(Item item, BuffId buffId);

	/// <summary>
	/// Base interface for item effect handlers (cards, equipment, etc.)
	/// </summary>
	public interface IItemEffectHandler
	{
		// Marker interface - no required methods
	}
}
