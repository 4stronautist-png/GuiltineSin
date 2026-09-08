using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_Dethrone)]
	public class Dragoon_DethroneOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int LineLength = 180;
		private const int LineWidth = 30;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var direction = originPos.GetDirection(farPos);
			var targetPos = originPos.GetRelative2D(direction, LineLength);
			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, targetPos))
				return;

			skill.Run(DragoonSkillHelper.AttackArea(caster, skill, originPos, targetPos, LineLength, LineWidth, 260, 1, this.AfterHit));
		}

		private void AfterHit(ICombatEntity caster, Skill skill, SkillHitInfo hit)
		{
			if (hit.HitInfo.Damage <= 0)
				return;

			hit.Target.StartBuff(BuffId.Dethrone_Debuff, skill.Level, 0, TimeSpan.FromSeconds(5), caster, skill.Id);
			hit.Target.StartBuff(BuffId.DethroneBoss_Debuff, skill.Level, 0, TimeSpan.FromSeconds(5), caster, skill.Id);
		}
	}
}
