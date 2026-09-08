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
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Rogue
{
	/// <summary>
	/// Handler for the Rogue skill Knife Throwing.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Rogue_KnifeThrowing)]
	public class Rogue_KnifeThrowingOverride : IGroundSkillHandler
	{
		private const float BackAttackAngle = 90f;
		private const float BackAttackDamageMultiplier = 1.0f;
		private const float DebuffDuration = 10000f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, caster.Position, caster.Direction, Position.Zero);

			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);

			skill.Run(this.Attack(skill, caster, originPos, farPos));
		}

		private async Task Attack(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 120, width: 15, angle: 10f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			if (caster.IsAbilityActive(AbilityId.Rogue22))
			{
				splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 0, width: 60, angle: 10f);
				splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);
			}

			var hitDelay = TimeSpan.FromMilliseconds(200);
			var aniTime = TimeSpan.FromMilliseconds(400);
			var skillHitDelay = TimeSpan.Zero;

			await skill.Wait(hitDelay);

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var hits = new List<SkillHitInfo>();

			foreach (var target in targets.LimitBySDR(caster, skill))
			{
				var modifier = SkillModifier.MultiHit(2);

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);

				if (caster.IsBehind(target, BackAttackAngle) || modifier.ForcedBackAttack)
				{
					skillHitResult.Damage *= (1 + BackAttackDamageMultiplier);
					Send.ZC_NORMAL.PlayTextEffect(target, caster, "SHOW_CUSTOM_TEXT", 50, "Back Attack!");
				}

				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);
				skillHit.HitEffect = HitEffect.Impact;

				hits.Add(skillHit);
			}

			Send.ZC_SKILL_HIT_INFO(caster, hits);

			SkillResultTargetBuff(caster, skill, BuffId.KnifeThrowing_Debuff, skill.Level, 0f, DebuffDuration, 1, 100, -1, hits);

			caster.StartBuff(BuffId.Cloaking_Buff, 0, 0, TimeSpan.FromSeconds(3), caster, SkillId.Scout_Cloaking);
		}
	}
}
