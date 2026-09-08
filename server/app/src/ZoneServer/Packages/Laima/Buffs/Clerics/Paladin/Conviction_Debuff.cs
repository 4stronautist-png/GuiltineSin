using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handle for the Conviction_Debuff debuff, which increases damage taken
	/// from elemental attacks.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Conviction_Debuff)]
	public class Conviction_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Conviction_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Conviction_Debuff, out var buff))
				return;

			// Get the attack attribute (override or skill's default)
			var attackAttribute = modifier.AttackAttribute == AttributeType.None ? skill.Data.Attribute : modifier.AttackAttribute;

			// Check if the incoming attack is of elemental type (Fire, Ice, Lightning, Earth, Poison)
			if (attackAttribute == AttributeType.Fire ||
			attackAttribute == AttributeType.Ice ||
			attackAttribute == AttributeType.Lightning ||
			attackAttribute == AttributeType.Earth ||
			attackAttribute == AttributeType.Poison)
			{
				// Increase damage taken from elemental type damage by 20% + 3% * SkillLv
				// buff.NumArg1 contains the skill level
				modifier.DamageMultiplier += 0.20f + (buff.NumArg1 * 0.03f);
			}
		}
	}
}
