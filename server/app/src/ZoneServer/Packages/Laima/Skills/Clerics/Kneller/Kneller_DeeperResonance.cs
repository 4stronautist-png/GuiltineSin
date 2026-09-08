using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Kneller
{
	[Package("laima")]
	[SkillHandler(SkillId.Kneller_DeeperResonance_Cleric)]
	public class Kneller_DeeperResonance_ClericOverride : IPassiveSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster)
		{
		}
	}
}
