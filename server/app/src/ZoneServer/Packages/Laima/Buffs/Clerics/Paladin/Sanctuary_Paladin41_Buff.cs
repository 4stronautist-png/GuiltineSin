using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handle for the Sanctuary: Liberty(Physical), * Increases Physical Defense{nl}* Immune to debuff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Sanctuary_Paladin41_Buff)]
	public class Sanctuary_Paladin41_BuffOverride : BuffHandler
	{
		public override void OnStart(Buff buff)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
