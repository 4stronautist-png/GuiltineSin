using System;
using System.Collections.Generic;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Wizard
{
	/// <summary>
	/// Handler for the Wizard skill Energy Bolt.
	/// </summary>
	[SkillHandler(SkillId.Wizard_EnergyBolt)]
	public class Wizard_EnergyBolt : ITargetSkillHandler
	{
		/// <summary>
		/// Handles the skill, attacking the targets.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="target"></param>
		public void Handle(Skill skill, ICombatEntity caster, ICombatEntity target)
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
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill, null);
				return;
			}

			if (!caster.InSkillUseRange(skill, target))
			{
				caster.ServerMessage(Localization.Get("Too far away."));
				return;
			}

			var aniTime = TimeSpan.FromMilliseconds(50);
			var hitDelay = skill.Properties.HitDelay;

			var splashArea = new Circle(target.Position, skill.Properties.GetFloat(PropertyName.SplRange));
			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);

			var skillHits = new List<SkillHitInfo>();
			var sharedForceId = ForceId.GetNew();

			foreach (var t in targets.LimitBySDR(caster, skill))
			{
				var skillHitResult = SCR_SkillHit(caster, t, skill, SkillModifier.MultiHit(2));

				var skillHit = new SkillHitInfo(caster, t, skill, skillHitResult, aniTime, hitDelay);
				skillHit.HitEffect = HitEffect.ImpactHard;
				skillHit.ForceId = sharedForceId;

				skillHit.ApplyDamage();
				skillHit.ApplyKnockBack();

				skillHits.Add(skillHit);
			}

			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill, skillHits);
		}
	}
}
