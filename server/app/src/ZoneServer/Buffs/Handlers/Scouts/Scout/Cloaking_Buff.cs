using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Scout
{
	/// <summary>
	/// Handler for the Cloaking buff.
	/// </summary>
	[BuffHandler(BuffId.Cloaking_Buff)]
	public class Cloaking_Buff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.NumArg2 <= 0)
				return;

			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, buff.NumArg2);

			if (buff.Target is Character character)
				Send.ZC_MOVE_SPEED(character);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);

			if (buff.Target is Character character)
				Send.ZC_MOVE_SPEED(character);
		}

		/// <summary>
		/// Applies the buff's effects during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Cloaking_Buff)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Cloaking_Buff, out var buff))
				return;

			modifier.DamageMultiplier -= 0.25f;
		}
	}
}
