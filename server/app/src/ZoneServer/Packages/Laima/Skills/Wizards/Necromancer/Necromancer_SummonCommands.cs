using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Necromancer
{
	[Package("laima")]
	[SkillHandler(SkillId.Common_ForcedAttack)]
	public class Necromancer_SummonForceAttackOverride : IForceSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity designatedTarget)
		{
			if (caster is not Character character)
				return;

			skill.IncreaseOverheat();
			Send.ZC_NORMAL.SkillTargetAnimation(caster, skill, caster.Direction, 1);
			Send.ZC_SKILL_FORCE_TARGET(caster, designatedTarget, skill);

			if (designatedTarget == null)
			{
				caster.ServerMessage(Localization.Get("No target specified."));
				return;
			}

			NecromancerSkillHelper.OrderAttack(skill, character, designatedTarget);
		}
	}

	[Package("laima")]
	[SkillHandler(SkillId.Common_ForcedAttackCancel)]
	public class Necromancer_SummonCancelAttackOverride : ISelfSkillHandler, IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			this.Execute(skill, caster);
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			this.Execute(skill, caster);
		}

		private void Execute(Skill skill, ICombatEntity caster)
		{
			if (caster is not Character character)
				return;

			skill.IncreaseOverheat();
			NecromancerSkillHelper.OrderCancelAttackAllSummons(character);
		}
	}

	[Package("laima")]
	[SkillHandler(SkillId.Common_SummonRemove)]
	public class Necromancer_SummonReleaseOverride : ISelfSkillHandler, IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			this.Execute(skill, caster);
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			this.Execute(skill, caster);
		}

		private void Execute(Skill skill, ICombatEntity caster)
		{
			if (caster is not Character character)
				return;

			skill.IncreaseOverheat();
			Send.ZC_NORMAL.SkillTargetAnimation(caster, skill, caster.Direction, 1);
			NecromancerSkillHelper.ReleaseNecromancerSummons(character);
		}
	}
}
