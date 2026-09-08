using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Oracle
{
	/// <summary>
	/// Handle for the Foretell buff, applied to friendly targets
	/// inside the Foretell pad area.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Foretell_Buff)]
	public class Oracle_Foretell_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
