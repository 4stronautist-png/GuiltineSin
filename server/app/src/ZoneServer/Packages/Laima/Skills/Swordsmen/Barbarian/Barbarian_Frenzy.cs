using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Buffs.Handlers.Swordsman.Barbarian;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Barbarian
{
	/// <summary>
	/// Handler for the passive Barbarian skill Frenzy.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Barbarian_Frenzy)]
	public class Barbarian_FrenzyOverride : ISkillHandler
	{
		/// <summary>
		/// Handles the offensive aspects of the Barbarian's passives.
		/// 1. Applies the Frenzy damage bonus (+5% per stack).
		/// 2. Applies the non-plate armor bonus (+50% damage) to specific skills.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, SkillId.Barbarian_Frenzy)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (attacker is not Character character)
				return;

			// Apply the +5% damage bonus per Frenzy stack
			if (character.TryGetBuff(BuffId.Frenzy_Buff, out var frenzyBuff))
			{
				modifier.DamageMultiplier += frenzyBuff.OverbuffCounter * 0.05f;
			}
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, SkillId.Barbarian_Frenzy)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetSkill(SkillId.Barbarian_Frenzy, out var frenzySkill))
				return;

			// Calculate max stacks: 6 + (SkillLevel - 1) * (14.0 / 9.0)
			// Lv 1: 6 stacks, Lv 10: 20 stacks
			var maxStacks = (int)Math.Round(6 + (frenzySkill.Level - 1) * (14.0 / 9.0));
			maxStacks = Math.Max(6, Math.Min(20, maxStacks));

			var buffInstanceDuration = TimeSpan.FromSeconds(10);

			// Gain a stack on every hit
			target.StartBuff(BuffId.Frenzy_Buff, maxStacks, 1, buffInstanceDuration, target, SkillId.Barbarian_Frenzy);
		}
	}
}
