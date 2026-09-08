using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handle for the Sanctuary: Weaken, Cannot receive Sanctuary: Liberty..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Sanctuary_Debuff)]
	public class Sanctuary_DebuffOverride : BuffHandler
	{
		public override void OnStart(Buff buff)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
