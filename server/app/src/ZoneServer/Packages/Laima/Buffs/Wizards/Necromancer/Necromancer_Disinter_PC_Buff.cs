using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Necromancer
{
	/// <summary>
	/// Handle for the Disinter, Positive effects are on your Skeleton summons..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Disinter_PC_Buff)]
	public class Necromancer_Disinter_PC_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
