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
	[PadHandler(PadName.Elemental_Burst_Pad_Fire)]
	public class Elemental_Burst_Pad_FireOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler
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

			PadTargetDamage(pad, initiator, out var skillHit, RelationType.Enemy, 1f, 0, 0);

			if (skillHit != null && skillHit.HitInfo.Damage > 0)
			{
				var burnDamage = Math.Max(1, (int)(skillHit.HitInfo.Damage * 0.1f));
				initiator.StartBuff(BuffId.Fire, skill.Level, burnDamage, TimeSpan.FromSeconds(5), creator);
			}
		}
	}
}
