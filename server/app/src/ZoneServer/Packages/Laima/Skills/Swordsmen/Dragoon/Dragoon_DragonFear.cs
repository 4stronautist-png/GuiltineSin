using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_DragonFear)]
	public class Dragoon_DragonFearOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.IsBuffActive(BuffId.DragoonHelmet_Buff))
			{
				caster.ServerMessage(Localization.Get("Requires Dragonoid."));
				Send.ZC_SKILL_DISABLE(caster);
				return;
			}

			if (!DragoonSkillHelper.StartSelfSkill(skill, caster, originPos))
				return;

			caster.StartBuff(BuffId.DragonFear_Debuff, skill.Level, 0, TimeSpan.FromMinutes(30), caster, skill.Id);
		}
	}
}
