using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.Skills.Handlers;

namespace GuiltineSin.Zone.Skills.HandlersOverrides.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Swordman skill Bear.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Swordman_Bear)]
	public class Swordman_BearOverride : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill, applying a buff to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="dir"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var duration = TimeSpan.FromSeconds(300);
			caster.StartBuff(BuffId.Bear_Buff, skill.Level, 0, duration, caster, skill.Id);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, originPos);
		}
	}
}
