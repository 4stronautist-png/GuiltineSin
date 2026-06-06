using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Swordsmen.Dragoon
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
