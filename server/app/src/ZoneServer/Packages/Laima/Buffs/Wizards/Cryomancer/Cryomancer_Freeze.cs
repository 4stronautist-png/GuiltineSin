using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Frozen, Frozen solid..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Cryomancer_Freeze)]
	public class Cryomancer_FreezeOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			if (target.Faction == FactionType.IceWall)
				return;

			target.AddState(StateType.Frozen);
			Send.ZC_SHOW_EMOTICON(target, "I_emo_freeze", buff.Duration);
			Send.ZC_NORMAL.StatusEffect(target, (int)buff.RemainingDuration.TotalMilliseconds, "Freeze", "Cryomancer_Freeze");
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			if (target.Faction == FactionType.IceWall)
				return;

			target.RemoveState(StateType.Frozen);
		}
	}
}
