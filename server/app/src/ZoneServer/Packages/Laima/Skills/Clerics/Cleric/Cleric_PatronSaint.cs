using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Cleric
{
	/// <summary>
	/// Handler for the Cleric skill Cure.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Cleric_PatronSaint)]
	public class Cleric_PatronSaintOverride : IGroundSkillHandler
	{
		private const int BuffDurationSeconds = 300;
		private const float AbilityBonus = 0.005f;

		/// <summary>
		/// Handles skill, damaging targets.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var healBonus = 0.15f + skill.Level * 0.03f;

			var byAbility = 1f;
			if (caster.TryGetActiveAbilityLevel(AbilityId.Cleric10, out var abilityLevel))
				byAbility += abilityLevel * AbilityBonus;
			healBonus *= byAbility;

			caster.StartBuff(BuffId.PatronSaint_Buff, skill.Level, healBonus, TimeSpan.FromSeconds(BuffDurationSeconds), caster);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);
		}
	}
}
