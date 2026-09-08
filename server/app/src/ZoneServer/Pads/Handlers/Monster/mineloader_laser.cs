using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.Handlers
{
	/// <summary>
	/// Handler for the Mineloader laser pad. Deals damage while inside
	/// and moves towards the target position.
	/// </summary>
	[PadHandler(PadName.mineloader_laser)]
	public class mineloader_laser : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		private const float PadDuration = 4300f;
		private const float MoveDistance = 200f;
		private const float MoveSpeed = MoveDistance / (PadDuration / 1000f);

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(150f);
			pad.SetUpdateInterval(500);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(PadDuration);

			if (skill.Vars.TryGet<Position>("GuiltineSin.Pad.TargetPos", out var targetPos))
			{
				pad.SetDestPos(targetPos, MoveSpeed, 0, false);
			}
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;

			PadDamageEnemy(pad, 1f, 0, -1, "None", 1, 0f, 0f);
		}
	}
}
