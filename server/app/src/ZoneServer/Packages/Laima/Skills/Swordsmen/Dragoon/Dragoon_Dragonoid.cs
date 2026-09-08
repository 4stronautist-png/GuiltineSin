using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_DragoonHelmet)]
	public class Dragoon_DragonoidOverride : ISelfSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!DragoonSkillHelper.StartSelfSkill(skill, caster, originPos))
				return;

			this.Apply(caster, skill);
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, farPos))
				return;

			this.Apply(caster, skill);
		}

		private void Apply(ICombatEntity caster, Skill skill)
		{
			if (caster.IsBuffActive(BuffId.DragoonHelmet_Buff))
			{
				caster.RemoveBuff(BuffId.DragoonHelmet_Buff);
				return;
			}

			caster.StartBuff(BuffId.DragoonHelmet_Buff, skill.Level, 0, TimeSpan.Zero, caster, skill.Id);
		}
	}
}
