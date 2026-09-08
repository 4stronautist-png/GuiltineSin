using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Falconer
{
	/// <summary>
	/// Handle for the Aiming, Aiming.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Aiming_Hawk_Buff)]
	public class Falconer_Aiming_Hawk_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
