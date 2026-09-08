using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Bokor
{
	/// <summary>
	/// Handler for the Dark Force buff used by Bokor class.
	/// This buff accumulates stacks (up to 20) which are consumed by certain Bokor skills.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.PowerOfDarkness_Buff)]
	public class PowerOfDarkness_BuffOverride : BuffHandler
	{
		/// <summary>
		/// Called when the buff is first applied or refreshed.
		/// </summary>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		/// <summary>
		/// Called when the buff expires or is removed.
		/// </summary>
		public override void OnEnd(Buff buff)
		{
		}

		/// <summary>
		/// Called periodically while the buff is active (every UpdateTime interval).
		/// </summary>
		public override void WhileActive(Buff buff)
		{
		}
	}
}
