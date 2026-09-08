using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Archer
{
	/// <summary>
	/// Handler for the Archer skill Concentration.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Archer_Concentration)]
	public class Archer_ConcentrationOverride : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill, applying buff to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		/// <param name="target"></param>
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

			if (caster.TryGetActiveAbilityLevel(AbilityId.Archer39, out _))
				duration = TimeSpan.FromSeconds(5);

			// Due to the dynamic duration this skill has,
			// we need to always remove the buff before applying it.
			caster.RemoveBuff(BuffId.Concentration_Buff);
			caster.StartBuff(BuffId.Concentration_Buff, skill.Level, 0, duration, caster);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);
		}
	}
}
