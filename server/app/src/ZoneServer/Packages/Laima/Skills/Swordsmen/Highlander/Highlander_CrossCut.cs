using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using System.Linq;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsman.Highlander
{
	/// <summary>
	/// Handler for the Highlander skill Cross Cut.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Highlander_CrossCut)]
	public class Highlander_CrossCutOverride : IGroundSkillHandler
	{
		private const float BaseBleedDamage = 0.2f;
		private const float BleedDamagePerLevel = 0.01f;
		private const float BaseBleedDuration = 5000;
		private const float BleedDurationPerLevel = 1000;

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

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.Attack(skill, caster, originPos, farPos));
		}

		private async Task Attack(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var bleedDuration = BaseBleedDuration + skill.Level * BleedDurationPerLevel;

			if (caster.TryGetActiveAbilityLevel(AbilityId.Highlander34, out var abilityLevel))
				bleedDuration += abilityLevel * 1000;

			var hits = new List<SkillHitInfo>();

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 45, width: 30, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			var hitDelay = 175;
			var aniTime = 0;
			await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, hits);
			foreach (var hit in hits)
			{
				var damage = hit.HitInfo.Damage;
				var bleedDamage = damage * BaseBleedDamage;
				bleedDamage += damage * skill.Level * BleedDamagePerLevel;
				bleedDamage = Math.Max(1, bleedDamage);
				SkillResultTargetBuff(caster, skill, BuffId.HeavyBleeding, skill.Level, bleedDamage, bleedDuration, 1, 100, -1, hit);
			}

			hits.Clear();

			splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 45, width: 30, angle: 0);
			splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			hitDelay = 560;
			aniTime = 360;
			await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, hits);
			foreach (var hit in hits)
			{
				var damage = hit.HitInfo.Damage;
				var bleedDamage = damage * BaseBleedDamage;
				bleedDamage += damage * skill.Level * BleedDamagePerLevel;
				bleedDamage = Math.Max(1, bleedDamage);
				SkillResultTargetBuff(caster, skill, BuffId.HeavyBleeding, skill.Level, bleedDamage, bleedDuration, 1, 100, -1, hit);
			}
		}
	}
}
