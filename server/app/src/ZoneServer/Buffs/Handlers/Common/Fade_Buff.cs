using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.AI;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handle for the Fade Buff, which resets the target's threat levels.
	/// </summary>
	[BuffHandler(BuffId.Fade_Buff)]
	public class Fade_Buff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.Map.AlertAis(new HateResetAlert(buff.Target));
		}
	}
}
