using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Barbarian
{
	/// <summary>
	/// Handler for the Barbarian skill Savagery.
	/// </summary>
	[SkillHandler(SkillId.Barbarian_Savagery)]
	public class Barbarian_Savagery : ISelfSkillHandler
	{
		private readonly static TimeSpan BuffDuration = TimeSpan.FromSeconds(50);

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

			var criticalBonus = 0;

			if (caster.TryGetActiveAbilityLevel(AbilityId.Barbarian5, out var barbarian5Level))
				criticalBonus = 5 * barbarian5Level;

			caster.StartBuff(BuffId.Savagery_Buff, skill.Level, criticalBonus, BuffDuration, caster);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster, null);
		}
	}
}
