using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the OperHide Buff, which resets the target's threat levels.
	/// </summary>
	[BuffHandler(BuffId.OperHide)]
	public class OperHide : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.Map.AlertAis(new HateResetAlert(buff.Target));
		}
	}
}
