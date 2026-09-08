using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Scout
{
	/// <summary>
	/// Handler for the Scout skill Cloaking.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Scout_Cloaking)]
	public class Scout_CloakingOverride : ISelfSkillHandler
	{
		/// <summary>
		/// Handles skill, applying a buff to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="dir"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var seconds = 10 + skill.Level * 3;
			var duration = TimeSpan.FromSeconds(seconds);
			caster.StartBuff(BuffId.Cloaking_Buff, skill.Level, 0, duration, caster, skill.Id);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);
		}
	}
}
