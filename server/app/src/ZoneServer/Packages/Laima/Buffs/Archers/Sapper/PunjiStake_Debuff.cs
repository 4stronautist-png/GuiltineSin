using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Sapper
{
	/// <summary>
	/// Handler for the PunjiStake Debuff, which prevents movement.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.PunjiStake_Debuff)]
	public class PunjiStake_DebuffOverride : BuffHandler
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
