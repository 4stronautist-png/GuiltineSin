using System;
using System.Linq;
using System.Numerics;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors.Pads;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using GuiltineSin.Zone.Skills;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[Package("laima")]
	[PadHandler(PadName.Cleric_Zaibas)]
	public class Krivis_ZaibasOverride : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var skill = args.Skill;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(50f);
			pad.SetUpdateInterval(150);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(10000);
			pad.Trigger.MaxActorCount = 4;
			pad.Trigger.MaxUseCount = 20;
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			var targets = pad.Trigger.GetAttackableEntities(creator);

			if (targets == null || !targets.Any())
				return;

			var targetCount = pad.Trigger.MaxActorCount;
			foreach (var target in targets)
			{
				if (targetCount <= 0)
					break;

				var modifier = new SkillModifier();
				modifier.BonusMAtk = pad.NumArg2;

				var skillHitResult = SCR_SkillHit(creator, target, skill, modifier);
				var damage = skillHitResult.Damage;
				target.TakeSimpleHit(damage, creator, skill.Id);

				this.TryApplyStagger(pad, creator, target, skill);

				pad.PlayEffectToGround("F_cleric_zaibas_shot_rize", target.Position, 1f, 3000f, 0, 0);
				pad.PlayEffectToGround("F_cleric_zaibas_shot_ground", target.Position, 0.5f, 500f, 0f, 0);

				targetCount--;
			}

			// After hitting all targets we can, we consume one use
			pad.Trigger.IncreaseUseCount();
		}
		private void TryApplyStagger(Pad pad, ICombatEntity creator, ICombatEntity target, Skill skill)
		{
			if (!target.IsKnockdownable())
				return;

			var staggerKey = "GuiltineSin.Zaibas.Stagger." + target.Handle;
			var now = DateTime.Now;

			if (pad.Variables.TryGet<DateTime>(staggerKey, out var lastStagger) && (now - lastStagger).TotalMilliseconds < 1000)
				return;

			pad.Variables.Set(staggerKey, now);

			var staggerResult = new SkillHitResult { Damage = 0, Result = HitResultType.Hit };
			var skillHit = new SkillHitInfo(creator, target, skill, staggerResult);
			skillHit.KnockBackInfo = new KnockBackInfo(pad.Position, target, KnockBackType.Motion, 0, 10);
			skillHit.HitInfo.KnockBackType = KnockBackType.Motion;
			target.ApplyKnockback(creator, skill, skillHit);
			Send.ZC_SKILL_HIT_INFO(creator, skillHit);
		}
	}
}
