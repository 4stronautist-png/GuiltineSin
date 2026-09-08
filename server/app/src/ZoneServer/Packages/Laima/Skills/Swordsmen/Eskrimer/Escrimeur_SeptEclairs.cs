using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Eskrimer
{
	[Package("laima")]
	[SkillHandler(SkillId.Escrimeur_SeptEclairs)]
	public class Escrimeur_SeptEclairs : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!EskrimerSkillHelper.TryBeginGroundSkill(skill, caster, originPos, farPos, target))
				return;

			EskrimerSkillHelper.GrantToucher(caster, skill);
			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			await EskrimerSkillHelper.AttackForward(caster, skill, originPos, farPos, 78, 20, 160, 120);
		}
	}
}
