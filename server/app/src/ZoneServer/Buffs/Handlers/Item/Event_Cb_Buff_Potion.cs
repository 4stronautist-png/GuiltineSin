using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for Event_Cb_Buff_Potion, providing attack bonuses.
	/// </summary>
	[BuffHandler(BuffId.Event_Cb_Buff_Potion)]
	public class Event_Cb_Buff_Potion : BuffHandler
	{
		private const int AttackBonus = 500;
		private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(1);

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			target.PlayEffect("F_sys_expcard_normal", 2.5f);
			target.Properties.Modify(PropertyName.PATK_BM, AttackBonus);
			target.Properties.Modify(PropertyName.MATK_BM, AttackBonus);
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.RemainingDuration > MaxDuration)
			{
				buff.IncreaseDuration(MaxDuration);
			}
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;
			target.Properties.Modify(PropertyName.PATK_BM, -AttackBonus);
			target.Properties.Modify(PropertyName.MATK_BM, -AttackBonus);
		}
	}
}
