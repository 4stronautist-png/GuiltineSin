using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handler for CARD_Shield buff, which absorbs damage up to a certain value.
	/// Used by cards like Armaos (10% chance to create shield with [★ * 150] value for 10s).
	/// </summary>
	/// <remarks>
	/// NumArg1: Shield value (e.g., star level * 150)
	/// </remarks>
	[BuffHandler(BuffId.CARD_Shield)]
	public class CARD_Shield : BuffHandler, IBuffCombatDefenseAfterCalcHandler
	{
		private const string ShieldValueKey = "GuiltineSin.CARD_Shield.RemainingValue";

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var shieldValue = buff.NumArg1;
			buff.Vars.SetFloat(ShieldValueKey, shieldValue);
			Send.ZC_UPDATE_SHIELD(buff.Target, (long)shieldValue, 1);
		}

		public override void OnExtend(Buff buff)
		{
			// Reset shield to full value when buff is re-applied
			var shieldValue = buff.NumArg1;
			buff.Vars.SetFloat(ShieldValueKey, shieldValue);
			Send.ZC_UPDATE_SHIELD(buff.Target, (long)shieldValue, 1);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Vars.Remove(ShieldValueKey);
			Send.ZC_UPDATE_SHIELD(buff.Target, 0, 1);
		}

		public void OnDefenseAfterCalc(Buff buff, ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			var remainingShield = buff.Vars.GetFloat(ShieldValueKey);

			if (remainingShield <= 0)
			{
				target.RemoveBuff(BuffId.CARD_Shield);
				return;
			}

			var damageToAbsorb = Math.Min(remainingShield, skillHitResult.Damage);
			skillHitResult.Damage -= damageToAbsorb;
			remainingShield -= damageToAbsorb;

			buff.Vars.SetFloat(ShieldValueKey, remainingShield);
			Send.ZC_UPDATE_SHIELD(target, (long)remainingShield, 0);

			if (skillHitResult.Damage <= 0)
			{
				skillHitResult.Effect = HitEffect.SAFETY;
				skillHitResult.Result = HitResultType.Miss;
			}

			if (buff.Vars.GetFloat(ShieldValueKey) <= 0)
			{
				target.RemoveBuff(BuffId.CARD_Shield);
			}
		}
	}
}
