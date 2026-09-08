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
	[SkillHandler(SkillId.Dragoon_Serpentine)]
	public class Dragoon_SerpentineOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var length = 46;
			var width = 24;
			var visualFarPos = DragoonSkillHelper.GetForwardTargetPos(caster, originPos, length);

			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, visualFarPos))
				return;

			skill.Run(DragoonSkillHelper.AttackForward(caster, skill, originPos, length, width, 280, 7, this.AfterHit, lastHitExtraDelay: 180));
		}

		private void AfterHit(ICombatEntity caster, Skill skill, SkillHitInfo hit)
		{
			if (hit.HitInfo.Damage <= 0)
				return;

			hit.Target.StartBuff(BuffId.Serpentine_Debuff, skill.Level, 20, TimeSpan.FromSeconds(5), caster, skill.Id);
		}
	}
}
