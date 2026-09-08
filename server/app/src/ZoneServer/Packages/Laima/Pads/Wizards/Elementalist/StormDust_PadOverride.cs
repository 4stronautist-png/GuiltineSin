using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.Handlers.Elementalist
{
	[Package("laima")]
	[PadHandler(PadName.StormDust_Pad)]
	public class StormDust_PadOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		private const int KnockbackChance = 40;
		private const int FreezeChance = 10;

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(75f);
			pad.SetUpdateInterval(500);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(12500);
			pad.Trigger.MaxUseCount = 1;
			var value = 13;
			pad.Trigger.MaxActorCount = value;
			pad.Trigger.MaxConcurrentUseCount = value;
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

			PadTargetBuff(pad, initiator, RelationType.Enemy, 0, 0, BuffId.StormDust_Debuff, skill.Level, 0, 12500, 1, 100, false);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;

			if (!creator.IsEnemy(initiator))
				return;

			PadTargetBuffRemoveMonster(pad, initiator, RelationType.Enemy, 0, 0, BuffId.StormDust_Debuff);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			var targets = pad.Trigger.GetAttackableEntities(creator);
			foreach (var target in targets)
			{
				if (target.IsDead)
					continue;

				if (!target.IsEnemy(creator))
					continue;

				if (!target.IsBuffActive(BuffId.StormDust_Debuff))
					target.StartBuff(BuffId.StormDust_Debuff, TimeSpan.FromMilliseconds(12500));
				this.TryApplySwirlingEffect(pad, creator, target, skill);
				this.TryApplyFreeze(pad, creator, target, skill);
			}
		}

		private void TryApplySwirlingEffect(Pad pad, ICombatEntity creator, ICombatEntity target, Skill skill)
		{
			if (!target.IsKnockdownable())
				return;

			var rnd = RandomProvider.Get();
			if (rnd.Next(100) >= KnockbackChance)
				return;

			var directionToCenter = target.Position.GetDirection(pad.Position);
			var perpendicularDirection = directionToCenter.Right.AddDegreeAngle(60);

			var isKnockdown = rnd.Next(2) == 0;

			if (isKnockdown)
			{
				var kb = new KnockBackInfo(target, KnockBackType.KnockDown, 150, 60, perpendicularDirection);
				target.Position = kb.ToPosition;
				target.AddState(StateType.KnockedDown, kb.Time);
				Send.ZC_KNOCKDOWN_INFO(target, kb);
			}
			else
			{
				var kb = new KnockBackInfo(target, KnockBackType.KnockBack, 100, 10, perpendicularDirection);
				target.Position = kb.ToPosition;
				target.AddState(StateType.KnockedBack, kb.Time);
				Send.ZC_KNOCKBACK_INFO(target, kb);
			}
		}

		private void TryApplyFreeze(Pad pad, ICombatEntity creator, ICombatEntity target, Skill skill)
		{
			if (target.IsBuffActive(BuffId.Freeze))
				return;

			var rnd = RandomProvider.Get();
			if (rnd.Next(100) >= FreezeChance)
				return;

			target.StartBuff(BuffId.Freeze, skill.Level, 0, TimeSpan.FromSeconds(3), creator);
		}
	}
}
