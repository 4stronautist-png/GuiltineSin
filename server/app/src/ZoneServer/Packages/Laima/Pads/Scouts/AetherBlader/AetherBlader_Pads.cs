using System.Linq;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Zone.Network;
using Melia.Zone.Pads;
using Melia.Zone.Pads.Handlers;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Scouts.AetherBlader;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.Monsters;
using Melia.Zone.World.Actors.Pads;

namespace Melia.Zone.Pads.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[PadHandler(PadName.AetherBlader_Puddle_pad)]
	public class AetherBlader_PuddlePad : IEnterPadHandler, IUpdatePadHandler, IDestroyPadHandler
	{
		public void Entered(object sender, PadTriggerActorArgs args)
			=> this.ApplyDrenched(args);

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			if (args.Trigger is Pad pad)
				Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(args.Creator, PadName.AetherBlader_Puddle_pad, args.Skill, pad.Position, pad.Direction, pad.NumArg1, pad.NumArg2, pad.Handle, pad.NumArg3, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			foreach (var target in args.Trigger.Trigger.GetAttackableEntities(args.Creator).Limit(args.Trigger.Trigger.MaxActorCount))
				target.StartBuff(BuffId.Wet_Debuff, args.Skill.Level, 0, AetherBladerSkillHelper.DrenchedDuration, args.Creator, args.Skill.Id);
		}

		private void ApplyDrenched(PadTriggerActorArgs args)
		{
			if (args.Initiator is ICombatEntity target && args.Creator.CanDamage(target))
				target.StartBuff(BuffId.Wet_Debuff, args.Skill.Level, 0, AetherBladerSkillHelper.DrenchedDuration, args.Creator, args.Skill.Id);
		}
	}

	[Package("laima")]
	[PadHandler(PadName.AetherBlader_FrostShatter, PadName.AetherBlader_FrostShatter_pad)]
	public class AetherBlader_FrostShatterPad : IUpdatePadHandler, IDestroyPadHandler
	{
		public void Destroyed(object sender, PadTriggerArgs args)
		{
			if (args.Trigger is Pad pad)
				Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(args.Creator, pad.Name, args.Skill, pad.Position, pad.Direction, pad.NumArg1, pad.NumArg2, pad.Handle, pad.NumArg3, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var targets = args.Trigger.Trigger.GetAttackableEntities(args.Creator).Limit(args.Trigger.Trigger.MaxActorCount).ToList();
			AetherBladerSkillHelper.DealHits(args.Creator, args.Skill, targets, target =>
				AetherBladerSkillHelper.BuildModifier(args.Skill, target, AetherBladerSkillHelper.GetTideCallFinalDamageBonus(args.Creator)));

			foreach (var target in targets)
			{
				if (target.IsBuffActive(BuffId.Wet_Debuff))
					target.StartBuff(BuffId.FrostSatter_Debuff, args.Skill.Level, 0, AetherBladerSkillHelper.FrozenDuration, args.Creator, args.Skill.Id);
			}
		}
	}
}
