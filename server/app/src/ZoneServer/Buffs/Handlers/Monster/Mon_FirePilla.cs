using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	[BuffHandler(BuffId.Mon_FirePilla)]
	public class Mon_FirePilla : DamageOverTimeBuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			base.OnActivate(buff, activationType);

			if (activationType == ActivationType.Start)
				Send.ZC_SHOW_EMOTICON(buff.Target, "I_emo_flame", buff.Duration);
		}

		protected override HitType GetHitType(Buff buff)
		{
			return HitType.Fire;
		}
	}
}
