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
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using GuiltineSin.Zone.Skills.SplashAreas;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Corsair skill Quick and Dead (Pistol Shot).
	/// Rapid-fire pistol attack dealing 4 hits to targets in range.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Corsair_PistolShot)]
	public class Corsair_PistolShotOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.HandleSkill(skill, caster, farPos));
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position farPos)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			var hits = new List<SkillHitInfo>();

			var splashParam = skill.GetSplashParameters(caster, caster.Position, farPos, length: 80f, width: 25f, angle: 110);
			var splashArea = skill.GetSplashArea(SplashType.Fan, splashParam);
			var aoeTargets = caster.Map.GetAttackableEnemiesIn(caster, splashArea)
				.OrderByDescending(t => t.IsBuffActive(BuffId.IronHooked))
				.LimitBySDR(caster, skill)
				.ToList();

			var hitCount = 8;
			for (var i = 0; i < hitCount; i++)
			{
				foreach (var target in aoeTargets)
				{
					if (target.IsDead)
						continue;

					var modifier = SkillModifier.Default;

					if (target.IsBuffActive(BuffId.IronHooked))
						modifier.CritRateMultiplier += 2.0f;

					var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
					target.TakeDamage(skillHitResult.Damage, caster);

					var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, TimeSpan.Zero, TimeSpan.Zero);
					skillHit.HitEffect = HitEffect.Impact;
					hits.Add(skillHit);

					Send.ZC_HIT_INFO(caster, target, skillHit.HitInfo);
				}

				if (i < hitCount - 1)
					await skill.Wait(TimeSpan.FromMilliseconds(60));
			}
		}
	}
}
