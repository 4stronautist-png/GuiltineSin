using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Falconer
{
	/// <summary>
	/// Handle for the Circling, AoE defense rate fixed to 1.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Circling_Buff)]
	public class Falconer_Circling_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
