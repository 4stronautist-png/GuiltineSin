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
	/// Handle for the Shock Absorption, Creates a protective barrier that
	/// temporarily absorbs incoming damage.
	/// </summary>
	[BuffHandler(BuffId.Mon_Shield)]
	public class Mon_Shield : BuffHandler, IBuffCombatDefenseAfterCalcHandler
	{
		private const string ShieldValueKey = "GuiltineSin.Monster.Shield";

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var shield = (int)MathF.Floor(buff.Target.MaxHp * 0.10f);
			buff.Vars.SetInt(ShieldValueKey, shield);
			Send.ZC_UPDATE_SHIELD(buff.Target, shield);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Vars.Remove(ShieldValueKey);
			Send.ZC_UPDATE_SHIELD(buff.Target, 0);
		}

		public void OnDefenseAfterCalc(Buff buff, ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			var remainingShield = buff.Vars.GetInt(ShieldValueKey);

			if (remainingShield <= 0)
			{
				target.RemoveBuff(BuffId.Mon_Shield);
				return;
			}

			var damageToAbsorb = Math.Min(remainingShield, (int)skillHitResult.Damage);
			skillHitResult.Damage -= damageToAbsorb;
			remainingShield -= damageToAbsorb;

			buff.Vars.SetInt(ShieldValueKey, remainingShield);
			Send.ZC_UPDATE_SHIELD(target, remainingShield);

			if (skillHitResult.Damage <= 0)
			{
				skillHitResult.Effect = HitEffect.SAFETY;
				skillHitResult.Result = HitResultType.Miss;
			}

			if (remainingShield <= 0)
			{
				target.RemoveBuff(BuffId.Mon_Shield);
			}
		}
	}
}
