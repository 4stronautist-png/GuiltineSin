using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.Scripting.AI;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Skills.Handlers.Barbarian
{
	/// <summary>
	/// Handler for the Barbarian skill Warcry
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Barbarian_Warcry)]
	public class Barbarian_WarcryOverride : ISelfSkillHandler
	{
		/// <summary>
		/// Handles skill, debuffing nearby enemies
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

			Send.ZC_SKILL_READY(caster, skill, originPos, Position.Zero);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);

			var splashParam = skill.GetSplashParameters(caster, caster.Position, caster.Position, length: 0, width: 250, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);

			var maxTargets = 4 + skill.Level;
			if (caster.TryGetActiveAbilityLevel(AbilityId.Barbarian1, out var barbarian1Level))
				maxTargets += barbarian1Level;

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea, maxTargets);

			var debuffDuration = 20;
			if (caster.TryGetActiveAbilityLevel(AbilityId.Barbarian2, out var barbarian2Level))
				debuffDuration += barbarian2Level * 2;

			foreach (var target in targets)
			{
				target.StartBuff(BuffId.Warcry_Debuff, skill.Level, 0, TimeSpan.FromSeconds(debuffDuration), caster);

				// Pull enemy hate/aggro
				if (target.Components.TryGet<AiComponent>(out var aiComponent))
				{
					// Reset hate and simulate a hit to build threat on the caster
					aiComponent.Script.QueueEventAlert(new HateResetAlert(caster));
					aiComponent.Script.QueueEventAlert(new HitEventAlert(target, caster, 0));
				}
			}
		}
	}
}
