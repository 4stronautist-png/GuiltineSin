using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Handler for the Wugushi skill Wugong Gu.
	/// </summary>
	[SkillHandler(SkillId.Wugushi_WugongGu)]
	public class Wugushi_WugongGu : ITargetSkillHandler
	{
		/// <summary>
		/// Handles skill, damages targets and apply a debuff.
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
			caster.SetAttackState(true);

			if (target == null)
			{
				// TODO: Skill_42 not implemented
				//Send.ZC_NORMAL.Skill_42(caster, skill.Id, caster.Direction, ForceId.GetNew());
				Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill, null);
				// TODO: Skill_43 not implemented
				//Send.ZC_NORMAL.Skill_43(caster);
				return;
			}

			caster.TurnTowards(target.Position);

			var aniTime = TimeSpan.FromMilliseconds(600);
			var skillHitResult = SCR_SkillHit(caster, target, skill);

			target.TakeDamage(skillHitResult.Damage, caster);

			var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, TimeSpan.Zero);

			Send.ZC_SKILL_READY(caster, skill, caster.Position, caster.Position);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, target.Handle, caster.Position, caster.Direction, target.Position);
			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill, skillHit);

			target.StartBuff(BuffId.Virus_Debuff, 0, 0, TimeSpan.FromSeconds(10), caster, skill.Id);
		}
	}
}
