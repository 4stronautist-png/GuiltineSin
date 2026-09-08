using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.Handlers.Wizards.Necromancer
{
	[Package("laima")]
	[PadHandler(PadName.Necromancer_CreateShoggoth_abil)]
	public class Necromancer_CreateShoggoth_abilOverride : ICreatePadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			pad.SetRange(70f);
			pad.SetUpdateInterval(750);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(60000 * creator.GetAbilityLevel(AbilityId.Necromancer3));
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			var buff = BuffId.Common_Rotten;
			if (creator.GetAbilityLevel(AbilityId.Necromancer16) > 0)
				buff = BuffId.Common_HighRotten;
			PadTargetBuff(pad, initiator, RelationType.Enemy, 0, 0, buff, 1, 0, 0, 1, 100, false);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			var buff = BuffId.Common_Rotten;
			if (creator.GetAbilityLevel(AbilityId.Necromancer16) > 0)
				buff = BuffId.Common_HighRotten;
			PadTargetBuffRemoveMonster(pad, initiator, RelationType.Enemy, 0, 0, buff);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;
		}
	}
}
