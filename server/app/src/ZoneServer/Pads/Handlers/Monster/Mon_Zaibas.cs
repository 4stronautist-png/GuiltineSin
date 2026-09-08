using System;
using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[PadHandler(PadName.Mon_Zaibas)]
	public class Mon_Zaibas : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(30f);
			pad.SetUpdateInterval(750);
			pad.Trigger.MaxActorCount = 1;
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(15000);
			pad.Trigger.MaxUseCount = 10;
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

			var modifier = new SkillModifier();

			var targetCount = pad.Trigger.MaxActorCount;
			foreach (var target in targets)
			{
				if (targetCount <= 0)
					break;

				var skillHitResult = SCR_SkillHit(creator, target, skill, modifier);
				var damage = skillHitResult.Damage;
				target.TakeSimpleHit(damage, creator, skill.Id);

				pad.PlayEffectToGround("F_cleric_zaibas_shot_rize", target.Position, 1f, 3000f, 0, 0);
				pad.PlayEffectToGround("F_cleric_zaibas_shot_ground", target.Position, 0.5f, 500f, 0f, 0);

				targetCount--;
			}

			pad.Trigger.IncreaseUseCount();
		}
	}
}
