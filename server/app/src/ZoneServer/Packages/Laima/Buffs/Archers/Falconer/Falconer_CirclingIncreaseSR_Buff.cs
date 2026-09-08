using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Falconer
{
	/// <summary>
	/// Handle for the Circling: Expand, AoE Attack Ratio increased..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.CirclingIncreaseSR_Buff)]
	public class Falconer_CirclingIncreaseSR_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
