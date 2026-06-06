using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_Gae_Bulg)]
	public class Dragoon_GaeBulgOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, farPos))
				return;

			skill.Run(DragoonSkillHelper.AttackCircle(caster, skill, farPos, 70, 360, 4, modify: DragoonSkillHelper.ApplySlowOrHoldBonus));
		}
	}
}
