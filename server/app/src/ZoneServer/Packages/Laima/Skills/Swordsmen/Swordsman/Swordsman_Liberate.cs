using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.HandlersOverrides.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Swordman skill Liberate.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Swordman_Liberate)]
	public class Swordman_LiberateOverride : ISelfSkillHandler
	{
		private const float ThreatPerLevel = 100f;

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
			var target = caster;

			// Normal duration
			var duration = TimeSpan.FromSeconds(30 * skill.Level);
			var abilityFlag = 0;

			if (caster.TryGetActiveAbilityLevel(AbilityId.Swordman31, out var a))
			{
				duration = TimeSpan.FromSeconds(12);
				abilityFlag = (int)AbilityId.Swordman31;
			}


			if (caster.TryGetActiveAbilityLevel(AbilityId.Swordman32, out var b))
			{
				duration = TimeSpan.FromSeconds(6);
				abilityFlag = (int)AbilityId.Swordman32;
			}

			target.StartBuff(BuffId.Liberate_Buff, skill.Level, abilityFlag, duration, caster);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, target);
		}
	}
}
