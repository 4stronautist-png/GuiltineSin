using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Event_Nru_Buff_Item, which plays an effect and has a max duration.
	/// </summary>
	[BuffHandler(BuffId.Event_Nru_Buff_Item)]
	public class Event_Nru_Buff_Item : BuffHandler
	{
		private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(1);

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.PlayEffect("F_sys_expcard_normal", 2.5f);
		}

		public override void WhileActive(Buff buff)
		{
			// The original script caps the remaining time at 1 hour (3,600,000 ms).
			if (buff.RemainingDuration > MaxDuration)
			{
				buff.IncreaseDuration(MaxDuration);
			}
		}
	}
}
