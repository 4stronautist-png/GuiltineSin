using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizard
{
	/// <summary>
	/// Handler for the Reflect Shield buff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ReflectShield_Buff)]
	public class ReflectShield_BuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.ReflectShield_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.ReflectShield_Buff, out var buff))
				return;

			var skillLevel = buff.NumArg1;
			var byAbility = 1 + (buff.NumArg2 * 0.05f);
			var multiplierReduction = (0.1f + skillLevel * 0.04f);
			multiplierReduction *= byAbility;

			// We originally reduced the damage directly from inside the combat
			// calculations, on BeforeBonuses, but setting the multiplier seems
			// much easier. Is this correct? Who knows.

			multiplierReduction = Math.Min(0.8f, multiplierReduction);

			modifier.DamageMultiplier *= (1f - multiplierReduction);

			var maxSp = target.Properties.GetFloat(PropertyName.MSP);
			var currentSp = target.Properties.GetFloat(PropertyName.SP);
			var spRate = 0.05f;
			var spConsume = maxSp * spRate;

			// Always uses remaining sp until zero
			spConsume = Math.Min(currentSp, spConsume);

			target.TrySpendSp(spConsume);
		}

		/// <summary>
		/// Called regularly, for effects that occur during while the
		/// buff is active.
		/// </summary>
		/// <param name="buff"></param>
		public override void WhileActive(Buff buff)
		{
			if (buff.Target is Character player)
			{
				if (player.Sp == 0)
					player.RemoveBuff(BuffId.ReflectShield_Buff);
			}
		}
	}
}
