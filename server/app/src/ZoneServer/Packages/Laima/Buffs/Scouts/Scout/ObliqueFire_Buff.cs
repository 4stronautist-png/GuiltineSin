using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scout
{
	/// <summary>
	/// Handler for Oblique Fire buff, which affects the target's movement
	/// speed.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ObliqueFire_Buff)]
	public class ObliqueFire_BuffOverride : BuffHandler
	{
		/// <summary>
		/// Applies the buff's effect before the damage calculation.
		/// </summary>
		/// <param name="buff"></param>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.ObliqueFire_Buff)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.ObliqueFire_Buff, out var buff))
				return;

			modifier.DamageMultiplier += buff.OverbuffCounter * 0.04f;
		}
	}
}
