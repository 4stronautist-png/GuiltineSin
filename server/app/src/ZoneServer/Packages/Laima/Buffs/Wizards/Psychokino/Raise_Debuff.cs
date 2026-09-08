using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Wizards.Psychokino
{
	/// <summary>
	/// Handle for the Raise, Restrained in the air..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Raise_Debuff)]
	public class Raise_DebuffOverride : BuffHandler
	{
		public override void OnStart(Buff buff)
		{
			var target = buff.Target;

			target.AddState(StateType.Raised);
			target.Vibrate(100, 1.5f, 10, 0.1f);
			target.FlyMath(70, 0.5f, 10);
			target.SetTempVar("RAISE_MOVETYPE", target.MoveType.ToString());
			target.MoveType = MoveType.Fly;
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			target.RemoveState(StateType.Raised);
			target.Vibrate(0.1f, 0, 0, 0);
			target.FlyMath(0, 0.1f, 0.5f);
			if (Enum.TryParse<MoveType>(target.GetTempVarStr("RAISE_MOVETYPE"), out var moveType))
				target.MoveType = moveType;
		}
	}
}
