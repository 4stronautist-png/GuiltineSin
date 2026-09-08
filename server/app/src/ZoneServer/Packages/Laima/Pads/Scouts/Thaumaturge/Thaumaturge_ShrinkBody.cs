using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Pads.Handlers;

namespace GuiltineSin.Zone.Packages.Laima.Pads.Scouts.Thaumaturge
{
	[Package("laima")]
	[PadHandler(PadName.Thaumaturge_ShrinkBody)]
	public class Thaumaturge_ShrinkBodyOverride : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(50f);
			pad.SetUpdateInterval(100);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(1000);
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

			if (pad.Variables.GetBool("GuiltineSin.Applied"))
				return;

			pad.Variables.SetBool("GuiltineSin.Applied", true);

			var maxTargets = (int)(3 + skill.Level * 0.5);
			var enemies = pad.Trigger.GetAttackableEntities(creator);
			var sorted = enemies.OrderBy(e => e.IsBuffActive(BuffId.ShrinkBody_Debuff) ? 1 : 0);

			var count = 0;
			var casterInt = (int)creator.Properties.GetFloat(PropertyName.INT);
			var hasThaumaturge4 = creator.IsAbilityActive(AbilityId.Thaumaturge4);

			foreach (var target in sorted)
			{
				if (count >= maxTargets)
					break;

				AddPadBuff(creator, target, pad, BuffId.ShrinkBody_Debuff, skill.Level, casterInt, 15000, 1, 100);
				count++;

				if (hasThaumaturge4)
				{
					var skillHitResult = SCR_SkillHit(creator, target, skill);
					target.TakeDamage(skillHitResult.Damage, creator);

					var hitInfo = new HitInfo(creator, target, skillHitResult.Damage, HitResultType.Hit);
					Send.ZC_HIT_INFO(creator, target, hitInfo);
				}
			}
		}
	}
}
