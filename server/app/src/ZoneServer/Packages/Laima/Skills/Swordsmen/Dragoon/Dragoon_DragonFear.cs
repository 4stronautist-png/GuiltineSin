using System;
using Melia.Shared.Game.Const;
using Melia.Shared.L10N;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Swordsmen.Dragoon
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
