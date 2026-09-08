using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads.Handlers;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Pads.HandlersOverride.Wizards.Sage
{
	[Package("laima")]
	[PadHandler(PadName.Sage_HoleOfDarkness)]
	public class Sage_HoleOfDarknessOverride : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		private const float PullRange = 50f;
		private const float PullDistance = 30f;

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(PullRange);
			pad.SetUpdateInterval(500);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(5000);
			pad.Trigger.MaxActorCount = 10;
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
			var creator = args.Creator;
			var skill = pad.Skill;
			var padCenter = pad.Position;

			var targets = pad.Trigger.GetAttackableEntities(creator);
			if (!targets.Any())
				return;

			var hits = new List<SkillHitInfo>();

			foreach (var target in targets)
			{
				if (target == null || target.IsDead)
					continue;

				// Calculate damage
				var skillHitResult = SCR_SkillHit(creator, target, skill);
				var skillHit = new SkillHitInfo(creator, target, skill, skillHitResult);

				// Pull towards center using knockback
				if (target.IsKnockdownable())
				{
					var pullDirection = target.Position.GetDirection(padCenter);
					var pullFromPos = target.Position.GetRelative(pullDirection.Backwards, PullDistance);

					skillHit.KnockBackInfo = new KnockBackInfo(pullFromPos, target, KnockBackType.KnockBack, 80, 10);
					skillHit.KnockBackInfo.Speed = 1;
					skillHit.KnockBackInfo.VPow = 1;
					skillHit.HitInfo.KnockBackType = KnockBackType.KnockBack;

					target.ApplyKnockback(creator, skill, skillHit);
				}

				target.TakeDamage(skillHitResult.Damage, creator);
				hits.Add(skillHit);
			}

			if (hits.Count > 0)
				Send.ZC_SKILL_HIT_INFO(creator, hits);
		}
	}
}
