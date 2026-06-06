using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Zone.Buffs;
using Melia.Zone.Buffs.Base;
using Melia.Zone.Network;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Scouts.BlitzHunter;
using Melia.Zone.World.Actors;
using static Melia.Zone.Skills.SkillUseFunctions;

	namespace Melia.Zone.Buffs.Handlers.Scouts.BlitzHunter
	{
		[Package("laima")]
		[BuffHandler(BuffId.Lightningrod_Buff)]
		public class Lightningrod_BuffOverride : BuffHandler
		{
			private const int MaxTargets = 10;

			public override void OnActivate(Buff buff, ActivationType activationType)
				=> this.TriggerArea(buff);

			public override void WhileActive(Buff buff)
				=> this.TriggerArea(buff);

			private void TriggerArea(Buff buff)
			{
				if (buff.Target.IsDead)
					return;

				if (!buff.Target.TryGetSkill(buff.SkillId, out var skill))
					return;

				var areaPosition = buff.Target.Position;
				var areaRange = Math.Max(skill.Properties.GetFloat(PropertyName.MaxR), skill.Data.SplashRange);
				var targets = BlitzHunterSkillHelper.GetCircleTargets(buff.Target, skill, areaPosition, areaRange, MaxTargets);
				if (targets.Count == 0)
					return;

				foreach (var target in targets)
				{
					if (target == null || target.IsDead)
						continue;

					var skillHitResult = SCR_SkillHit(buff.Target, target, skill, SkillModifier.MultiHit(2));
					target.TakeDamage(skillHitResult.Damage, buff.Target);

					var hit = new HitInfo(buff.Target, target, skill, skillHitResult);
					Send.ZC_HIT_INFO(buff.Target, target, hit);
					Send.ZC_PLAY_SOUND(buff.Target, "skl_eff_blitzhunter_lightning_hit");
					Send.ZC_GROUND_EFFECT(buff.Target, target.Position, "GroundImpact_Lightningbolt_Blue_01", 0.7f, 0f, 0f, 0f);
				}
			}
		}
	}
