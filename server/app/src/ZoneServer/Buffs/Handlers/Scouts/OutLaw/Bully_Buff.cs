using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.OutLaw
{
	/// <summary>
	/// Buff handler for Bully Buff, which increases evasion
	/// and adds threat on successful evade.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[BuffHandler(BuffId.Bully_Buff)]
	public class Bully_Buff : BuffHandler
	{
		private const float DrBuffRateBase = 0.24f;
		private const float DrBuffRatePerLevel = 0.04f;
		private const float HatePerLevel = 3f;

		/// <summary>
		/// Starts buff, increasing dodge rate.
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var dr = buff.Target.Properties.GetFloat(PropertyName.DR);
			var skillLevel = buff.NumArg1;
			var rate = DrBuffRateBase + DrBuffRatePerLevel * skillLevel;
			var bonus = dr * rate;

			AddPropertyModifier(buff, buff.Target, PropertyName.DR_BM, bonus);
		}

		/// <summary>
		/// Ends the buff, resetting dodge rate.
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.DR_BM);
		}

		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Bully_Buff)]
		public void OnAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Bully_Buff, out var buff))
				return;

			if (skillHitResult.Result == HitResultType.Dodge && attacker.Components.TryGet<AiComponent>(out var component))
			{
				component.Script.QueueEventAlert(new HateIncreaseAlert(target, buff.NumArg1 * HatePerLevel));

				// Outlaw12 adds additional duration to the buff on successful evade
				// Note that it only applies to monster attacks, which is why
				// it's done after the AI script check
				if (target.TryGetActiveAbilityLevel(AbilityId.Outlaw12, out var level))
				{
					buff.ExtendDuration();
				}
			}
		}
	}
}
