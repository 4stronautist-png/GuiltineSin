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
	[SkillHandler(SkillId.Dragoon_Dragon_Soar)]
	public class Dragoon_DragonSoarOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var length = 46;
			var width = 28;
			var visualFarPos = DragoonSkillHelper.GetForwardTargetPos(caster, originPos, length);

			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, visualFarPos))
				return;

			var hits = caster.IsAbilityActive(AbilityId.Dragoon16) ? 10 : 5;
			skill.Run(DragoonSkillHelper.AttackForward(caster, skill, originPos, length, width, 240, hits, this.AfterHit, DragoonSkillHelper.ApplySlowOrHoldBonus));
		}

		private void AfterHit(ICombatEntity caster, Skill skill, SkillHitInfo hit)
		{
			if (hit.HitInfo.Damage > 0)
				hit.Target.StartBuff(BuffId.Common_Shock, skill.Level, 0, TimeSpan.FromSeconds(8), caster, skill.Id);
		}
	}
}
