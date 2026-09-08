using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Pads.Handlers;

namespace GuiltineSin.Zone.Packages.Laima.Pads.Scouts.Thaumaturge
{
	[Package("laima")]
	[PadHandler(PadName.Thaumaturge_SwellBody)]
	public class Thaumaturge_SwellBodyOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(50f);
			pad.SetUpdateInterval(100);
			pad.Trigger.MaxActorCount = (int)(3 + skill.Level * 0.5);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(10000);
			pad.Trigger.MaxUseCount = (int)(3 + skill.Level * 0.5);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!PadActivate(pad, initiator, RelationType.Enemy)) return;
			PadTargetBuff(pad, initiator, RelationType.Enemy, 0, -1, BuffId.SwellBody_Debuff, 1, 0, 5000 + skill.Level * 1000, 1, 100, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;
		}
	}
}
