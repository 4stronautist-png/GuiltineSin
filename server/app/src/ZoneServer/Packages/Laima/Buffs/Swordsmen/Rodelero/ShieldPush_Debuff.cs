using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Rodelero
{
	/// <summary>
	/// Handler for ShieldPush_Debuff (Unbalance).
	/// Prevents the target from moving but does not lock attacks.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ShieldPush_Debuff)]
	public class ShieldPush_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.AddState(StateType.Held);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.RemoveState(StateType.Held);
		}
	}
}
