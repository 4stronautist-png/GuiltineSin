using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Peltasta
{
	/// <summary>
	/// Contains code related to the Langort Block
	/// </summary>
	/// <remarks>
	/// This is completely identical to Momentary Block,
	/// but without the counter effect.
	/// </remarks>
	[BuffHandler(BuffId.Langort_BlkAbil)]
	public class Langort_BlkAbil : BuffHandler
	{
		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Langort_BlkAbil)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Langort_BlkAbil, out var buff))
				return;

			modifier.ForcedBlock = true;
		}
	}
}
