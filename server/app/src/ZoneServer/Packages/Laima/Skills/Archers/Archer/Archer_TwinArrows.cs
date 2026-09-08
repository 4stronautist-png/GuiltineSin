using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Archers.Ranger;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Archer
{
	/// <summary>
	/// Handles the Archer skill Twin Arrow.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Archer_TwinArrows)]
	public class Archer_TwinArrowsOverride : IForceSkillHandler
	{
		/// <summary>
		/// Handles the skill, do two consecutive hits on the enemy.
		/// </summary>
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
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill, null);
				return;
			}

			if (!caster.InSkillUseRange(skill, target))
			{
				caster.ServerMessage(Localization.Get("Too far away."));
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill, null);
				return;
			}

			var aniTime = TimeSpan.FromMilliseconds(45);
			var skillHitDelay = skill.Properties.HitDelay;

			// TODO: Add more 50% damage to enemies using cloth armor type

			var skillHitResult = SCR_SkillHit(caster, target, skill, SkillModifier.MultiHit(2));
			target.TakeDamage(skillHitResult.Damage, caster);

			var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);

			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill, skillHit);

			Ranger_CriticalShotOverride.TryActivateDoubleTake(skill, caster, target);
		}
	}
}
