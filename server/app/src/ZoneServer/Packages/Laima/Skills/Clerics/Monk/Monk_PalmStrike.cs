using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handler for the Monk skill Palm Strike.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Monk_PalmStrike)]
	public class Monk_PalmStrikeOverride : IGroundSkillHandler
	{
		protected TimeSpan AniTime { get; } = TimeSpan.FromMilliseconds(350);
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var targetHandle = target?.Handle ?? 0;
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 0, width: 55, angle: 10f);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);
			var hitDelay = 150;
			var aniTime = 350;

			await skill.Wait(TimeSpan.FromMilliseconds(hitDelay));

			if (caster.IsDead) return;

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var hits = new List<SkillHitInfo>();
			var skillHitDelay = skill.Properties.HitDelay;

			foreach (var target in targets.LimitBySDR(caster, skill))
			{
				if (!caster.IsEnemy(target))
					continue;

				var skillHitResult = SCR_SkillHit(caster, target, skill);
				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, TimeSpan.FromMilliseconds(aniTime), skillHitDelay);
				skillHit.HitEffect = HitEffect.Impact;

				if (skillHitResult.Damage > 0 && target.IsKnockdownable())
				{
					if (caster.IsAbilityActive(AbilityId.Monk3))
						skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, skillHit.Target, KnockBackType.KnockDown, 70, 90);
					else
						skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, skillHit.Target, KnockBackType.KnockDown, 180, 33);

					skillHit.HitInfo.KnockBackType = KnockBackType.KnockDown;
					target.ApplyKnockdown(caster, skill, skillHit);
				}

				hits.Add(skillHit);
			}

			Send.ZC_SKILL_HIT_INFO(caster, hits);
			caster.StartBuff(BuffId.HandKnife_Buff, 1f, 0f, TimeSpan.FromMilliseconds(2000f), caster);
		}
	}
}
