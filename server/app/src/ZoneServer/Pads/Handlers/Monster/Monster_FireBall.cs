using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[PadHandler(PadName.Monster_FireBall)]
	public class Monster_FireBall : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			pad.SetRange(15f);
			pad.SetUpdateInterval(400);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(30000);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			PadTargetBuffMon(pad, initiator, RelationType.Enemy, 0, 0, BuffId.UC_flame, 1, 0, 5000, 1, 5);
			PadTargetDamage(pad, initiator, RelationType.Enemy, 1f, -30000, 0);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

		}
	}
}
