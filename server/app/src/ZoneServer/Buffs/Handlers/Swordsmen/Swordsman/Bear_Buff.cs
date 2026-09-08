using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Bear buff.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[BuffHandler(BuffId.Bear_Buff)]
	public class Bear_Buff : BuffHandler
	{
		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Bear_Buff)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Bear_Buff, out var buff))
				return;

			var skillLevel = buff.NumArg1;
			var multiplierReduction = skillLevel * 0.02f;

			// Ability "Bear: Enhance"
			// Increases damage reducing effects by 0.5% per level
			if (target.TryGetActiveAbility(AbilityId.Swordman30, out var ability))
				multiplierReduction += ability.Level * 0.005f;

			// We originally reduced the damage directly from inside the
			// combat calculations, on AfterBonuses, but setting the
			// multiplier seems much easier. Is this correct? Who knows.

			modifier.DamageMultiplier -= multiplierReduction;
		}
	}
}
