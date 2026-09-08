using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads.Handlers;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.HandlersOverride.Wizards.Psychokino
{
	[Package("laima")]
	[PadHandler(PadName.Psychokino_Raise)]
	public class Psychokino_RaiseOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(50f);
			pad.SetUpdateInterval(1000);
			var life = 3000f + (200f * skill.Level);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(life);
			pad.Trigger.MaxUseCount = skill.Level;
			pad.Trigger.MaxConcurrentUseCount = 4 + (skill.Level * 2);
			var value = skill.Level;
			pad.Trigger.MaxActorCount = value;
			PadSelectPadKill(pad, PadName.HeavyGravity_PAD, 100f);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, false);
			foreach (var actor in pad.Trigger.GetActors<ICombatEntity>())
			{
				if (actor.TryGetBuff(BuffId.Raise_Debuff, out var buff)
					&& buff.Caster == creator)
					actor.RemoveBuff(buff.Id);
			}
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!creator.IsEnemy(initiator))
				return;

			if (pad.Trigger.AtCapacity)
				return;

			pad.Trigger.ActivateCount++;
			var duration = 3000 + (skill.Level * 200);
			PadTargetBuff(pad, initiator, RelationType.Enemy, 0, 0, BuffId.Raise_Debuff, 1, 0, duration, 1, 100, false);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!creator.IsEnemy(initiator))
				return;

			if (!initiator.IsBuffActive(BuffId.Raise_Debuff))
				return;

			pad.Trigger.ActivateCount--;
			PadTargetBuffRemove(pad, initiator, RelationType.All, 0, 0, BuffId.Raise_Debuff, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

		}
	}
}
