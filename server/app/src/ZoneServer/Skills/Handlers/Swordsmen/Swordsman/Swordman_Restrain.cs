using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Swordman skill Restrain
	/// </summary>
	[SkillHandler(SkillId.Swordman_Restrain)]
	public class Swordman_Restrain : ISelfSkillHandler
	{
		private const float StunChancePerLevel = 6f;

		/// <summary>
		/// Handles skill, applying the buff to the caster.
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

			var duration = TimeSpan.FromSeconds(30 + 3 * skill.Level);
			var stunChance = StunChancePerLevel * skill.Level;

			caster.StartBuff(BuffId.Restrain_Buff, skill.Level, stunChance, duration, caster);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster, null);
		}
	}
}
