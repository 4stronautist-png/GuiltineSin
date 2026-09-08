using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handler for the ResistElements_Buff, which reduces incoming elemental damage.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ResistElements_Buff)]
	public class ResistElements_BuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.ResistElements_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.ResistElements_Buff, out var buff))
				return;

			var attackAttribute = modifier.AttackAttribute == AttributeType.None ? skill.Data.Attribute : modifier.AttackAttribute;

			// Check if the incoming attack is elemental (Fire, Ice, Lightning, Earth, Poison)
			if (attackAttribute == AttributeType.Fire ||
				attackAttribute == AttributeType.Ice ||
				attackAttribute == AttributeType.Lightning ||
				attackAttribute == AttributeType.Earth ||
				attackAttribute == AttributeType.Poison)
			{
				// Base reduction: 15% + 1.5% * SkillLv
				var baseReduction = 0.15f + (buff.NumArg1 * 0.015f);

				// Paladin37 ability: 0.5% multiplier per ability level (1.5x at level 100)
				var abilityMultiplier = 1f;
				if (target.TryGetActiveAbilityLevel(AbilityId.Paladin37, out var abilityLevel))
					abilityMultiplier += abilityLevel * 0.005f;

				// Calculate final reduction, capped at 90%
				var finalReduction = Math.Min(0.90f, baseReduction * abilityMultiplier);

				// Reduce damage by the calculated percentage
				modifier.DamageMultiplier *= (1f - finalReduction);
			}
		}
	}
}
