using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_Dragontooth)]
	public class Dragoon_DragontoothOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int LineLength = 46;
		private const int LineWidth = 24;

		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			this.FinishDragontooth(skill, caster, originPos);
		}

		private void FinishDragontooth(Skill skill, ICombatEntity caster, Position originPos)
		{
			var visualFarPos = DragoonSkillHelper.GetForwardTargetPos(caster, originPos, LineLength);

			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, visualFarPos))
				return;

			skill.Run(DragoonSkillHelper.AttackForward(caster, skill, originPos, LineLength, LineWidth, 260, 6, modify: DragoonSkillHelper.ApplyDragontoothLongCanine));
		}
	}
}
