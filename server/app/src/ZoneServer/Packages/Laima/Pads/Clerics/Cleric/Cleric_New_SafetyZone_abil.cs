using System;
using System.Collections.Generic;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[Package("laima")]
	[PadHandler(PadName.Cleric_New_SafetyZone_abil)]
	public class Cleric_New_SafetyZone_abilOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		private const string HitCountVariableName = "GuiltineSin.SafetyZone.HitCount";

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = args.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(40f);
			pad.SetUpdateInterval(1000);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(30000);

			var hitCount = 6 + skill.Level;
			pad.Variables.SetInt(HitCountVariableName, hitCount);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			var targets = pad.Trigger.GetAlliedEntities(creator);
			if (pad.Trigger.Area.IsInside(creator.Position))
				targets.Add(creator);
			foreach (var target in targets)
				target.RemoveBuff(BuffId.SafetyZone_Buff);

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!initiator.IsAlly(creator))
				return;

			if (initiator.IsBuffActive(BuffId.SafetyZone_Buff))
				return;

			initiator.StartBuff(BuffId.SafetyZone_Buff, skill.Level, 0f, TimeSpan.Zero, creator);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var initiator = args.Initiator;
			initiator.RemoveBuff(BuffId.SafetyZone_Buff);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			var remainingHits = pad.Variables.GetInt(HitCountVariableName);
			if (remainingHits <= 0)
			{
				pad.Destroy();
				return;
			}
		}
	}
}
