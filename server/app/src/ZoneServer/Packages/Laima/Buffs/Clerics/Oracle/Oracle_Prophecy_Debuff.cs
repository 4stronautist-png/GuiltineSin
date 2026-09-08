using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Oracle
{
	/// <summary>
	/// Handle for the Prophecy, Receiving Holy damage.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Prophecy_Debuff)]
	public class Oracle_Prophecy_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
