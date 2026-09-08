using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.Handlers.Elementalist
{
	[Package("laima")]
	[PadHandler(PadName.Elemental_Burst_Pad_Ice)]
	public class Elemental_Burst_Pad_IceOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(20f);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(500);
			pad.Trigger.MaxActorCount = 5;
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!creator.IsEnemy(initiator))
				return;

			var damageRate = 1f;
			if (ZoneServer.Instance.World.IsPVP)
				damageRate = 0.5f;

			PadTargetDamage(pad, initiator, RelationType.Enemy, damageRate, 0, 0);
			initiator.StartBuff(BuffId.Freeze, skill.Level, 0, TimeSpan.FromSeconds(5), creator);
		}
	}
}
