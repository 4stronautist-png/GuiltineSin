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

namespace GuiltineSin.Zone.Pads.Handlers
{
	[Package("laima")]
	[PadHandler(PadName.Cleric_Zemina)]
	public class Cleric_ZeminaOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(100f);
			// Match statue lifetime: 15 + (skill.Level * 2) seconds
			pad.Trigger.LifeTime = TimeSpan.FromSeconds(15 + skill.Level * 2);
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

			PadTargetBuffAfterBuffCheck(pad, initiator, RelationType.Party, 0, 0, BuffId.Ausirine_Buff, BuffId.CarveZemina_Buff, skill.Level, 0, 0, 1, 100, false);
			PadTargetBuffCheckAbility(pad, initiator, RelationType.Party, AbilityId.Dievdirbys33, 0, 0, BuffId.CarveZemina_Abil_Buff, skill.Level, 0, 0, 1, 100);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			PadTargetBuffRemove(pad, initiator, RelationType.Party, 0, 0, BuffId.CarveZemina_Buff, false);
			PadTargetBuffRemove(pad, initiator, RelationType.All, 0, 0, BuffId.CarveZemina_Abil_Buff, false);
		}
	}
}
