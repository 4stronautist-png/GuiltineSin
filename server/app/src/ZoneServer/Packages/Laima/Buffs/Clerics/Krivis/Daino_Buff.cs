using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Daino, increases healing received.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Daino_Buff)]
	public class Daino_BuffOverride : BuffHandler
	{
	}
}
