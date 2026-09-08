using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Wizard
{
	/// <summary>
	/// Handler for the Wizard skill Earthquake.
	/// </summary>
	[SkillHandler(SkillId.Wizard_EarthQuake)]
	public class Wizard_EarthQuake : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill, damaging targets.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 50, width: 50, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);

			var targets = caster.Map.GetAttackableEntitiesIn(caster, splashArea);

			var aniTime = TimeSpan.FromMilliseconds(200);
			var hitDelay = skill.Properties.HitDelay;

			var skillHits = new List<SkillHitInfo>();

			foreach (var t in targets.LimitBySDR(caster, skill))
			{
				var modifier = SkillModifier.Default;

				// Buff "Lethargy"
				// Hits twice against enemies under the effect of [Lethargy]
				if (t.IsBuffActive(BuffId.Lethargy_Debuff))
					modifier.HitCount = 2;

				var skillHitResult = SCR_SkillHit(caster, t, skill, modifier);

				// Ability "Earthquake: Remove Knockdown"
				if (caster.IsAbilityActive(AbilityId.Wizard23))
					skillHitResult.KnockBack.Type = KnockBackType.None;

				var skillHit = new SkillHitInfo(caster, t, skill, skillHitResult, aniTime, hitDelay);

				skillHit.ApplyDamage();
				skillHit.ApplyKnockBack();

				skillHits.Add(skillHit);
			}

			var targetHandle = target?.Handle ?? 0;

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, skillHits);
		}
	}
}
