using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Versioning;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Wizard
{
	/// <summary>
	/// Handler for the Wizard skill Earthquake.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Wizard_EarthQuake)]
	public class Wizard_EarthQuakeOverride : IGroundSkillHandler, IDynamicCasted, ICancelSkillHandler
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

			// Attack targets
			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var aniTime = TimeSpan.FromMilliseconds(200);

			var skillHits = new List<SkillHitInfo>();

			var targetCount = 12;
			foreach (var t in targets)
			{
				if (targetCount == 0)
					break;

				var targetIsMoving = t.Components.TryGet<MovementComponent>(out var targetMovement) && targetMovement.IsMoving;

				var targetLethargic = t.IsBuffActive(BuffId.Lethargy_Debuff);

				var skillHitResult = SCR_SkillHit(caster, t, skill, SkillModifier.MultiHitIf(2, targetLethargic));
				t.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, t, skill, skillHitResult, aniTime, TimeSpan.Zero);

				if (skillHitResult.Damage > 0 && !caster.IsAbilityActive(AbilityId.Wizard23) && t.IsKnockdownable())
				{
					skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, t, KnockBackType.KnockDown, 400, 86);
					skillHit.HitInfo.KnockBackType = skill.Data.KnockDownHitType;
					t.ApplyKnockdown(caster, skill, skillHit);
				}

				skillHits.Add(skillHit);
				targetCount--;
			}

			var targetHandle = target?.Handle ?? 0;

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);

			if (Versions.Protocol > 500)
				Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, skillHits);
			else
			{
				Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);
				Send.ZC_SKILL_HIT_INFO(caster, skillHits);
			}
		}

		/// <summary>
		/// Behavior when the skill is cancelled.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		public void Handle(Skill skill, ICombatEntity caster)
		{
			// Method intentionally left empty.
		}
	}
}
