using System;
using System.Collections.Generic;
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
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Handler for the Wugushi skill Latent Venom.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Wugushi_LatentVenom)]
	public class Wugushi_LatentVenomOverride : IForceSkillHandler
	{
		private const int AniTimeMs = 300;
		private const int BaseLatentDurationMs = 40000;
		private const int StacksPerUse = 2;
		private const float SplashLength = 30f;
		private const float SplashWidth = 30f;
		private const float SplashAngle = 10f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.TurnTowards(target);
			caster.SetAttackState(true);

			if (target == null)
			{
				Send.ZC_NORMAL.SkillTargetAnimation(caster, skill, caster.Direction, 1);
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill);
				return;
			}

			if (!caster.InSkillUseRange(skill, target))
			{
				caster.ServerMessage(Localization.Get("Too far away."));
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill);
				return;
			}

			var skillHitResult = SCR_SkillHit(caster, target, skill);
			target.TakeDamage(skillHitResult.Damage, caster);

			var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, TimeSpan.Zero, TimeSpan.Zero);

			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill, skillHit);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos, target, skillHitResult));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos, ICombatEntity primaryTarget, SkillHitResult primaryHitResult)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: SplashLength, width: SplashWidth, angle: SplashAngle);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			var hits = new List<SkillHitInfo>();

			var primaryHit = new SkillHitInfo(caster, primaryTarget, skill, primaryHitResult, TimeSpan.FromMilliseconds(AniTimeMs), TimeSpan.Zero);
			hits.Add(primaryHit);

			await skill.Wait(TimeSpan.FromMilliseconds(AniTimeMs));

			var stacks = StacksPerUse;

			foreach (var hit in hits)
			{
				var target = hit.Target;
				if (target.IsDead)
					continue;

				if (hit.HitInfo.Damage <= 0)
					continue;

				var buff = target.StartBuff(BuffId.LatentVenom_Debuff, stacks, hit.HitInfo.Damage, TimeSpan.FromMilliseconds(BaseLatentDurationMs), caster);
				if (buff != null)
					buff.OverbuffCounter = 1;
			}
		}
	}
}
