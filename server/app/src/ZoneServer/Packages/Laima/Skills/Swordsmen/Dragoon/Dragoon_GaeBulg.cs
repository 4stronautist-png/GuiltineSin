using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
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
