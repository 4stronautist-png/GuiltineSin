using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Pads.Handlers.Clerics.Sadhu
{
	/// <summary>
	/// Handler for the Sadhu Moksha Pad, creates and disables the effect
	/// </summary>
	[Package("laima")]
	[PadHandler(PadName.Sadhu_Moksha_Pad)]
	public class Sadhu_Moksha_PadOverride : ICreatePadHandler, IDestroyPadHandler
	{
		/// <summary>
		/// Called when the pad is created.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(creator, pad, PadName.Sadhu_Moksha_Pad, -0.7853982f, 0, 100, true);
		}

		/// <summary>
		/// Called when the pad is destroyed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(creator, pad, PadName.Sadhu_Moksha_Pad, -0.7853982f, 0, 100, false);
		}
	}
}
