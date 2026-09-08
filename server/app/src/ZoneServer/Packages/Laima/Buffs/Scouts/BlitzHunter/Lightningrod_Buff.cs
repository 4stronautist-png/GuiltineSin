using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Scouts.BlitzHunter;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

	namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.BlitzHunter
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
