using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handle for the Stun Debuff. Unable to act. Increases minimum
	/// critical chance by +30% with incoming Pierce attacks.
	/// </summary>
	[BuffHandler(BuffId.Stun, BuffId.UC_stun)]
	public class Stun : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.AddState(StateType.Stunned);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.RemoveState(StateType.Stunned);
		}
	}
}
