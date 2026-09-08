using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Fire Pillar, Continuously receive Fire damage..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.FirePillar_Debuff)]
	public class FirePillar_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			Send.ZC_SHOW_EMOTICON(target, "I_emo_flame", buff.Duration);
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.Caster is ICombatEntity caster)
			{
				var target = buff.Target;
				var minMATK = caster.Properties.GetFloat(PropertyName.MINMATK);
				var maxMATK = caster.Properties.GetFloat(PropertyName.MAXMATK);
				var damage = (minMATK + maxMATK) / RandomProvider.Next(7, 10);

				var forceId = ForceId.GetNew();

				target.TakeDamage(damage, caster);

				var hitInfo = new HitInfo(caster, target, damage, HitResultType.Hit);
				hitInfo.ForceId = forceId;
				hitInfo.Type = HitType.Fire;

				Send.ZC_HIT_INFO(caster, target, hitInfo);
			}
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

		}
	}
}
