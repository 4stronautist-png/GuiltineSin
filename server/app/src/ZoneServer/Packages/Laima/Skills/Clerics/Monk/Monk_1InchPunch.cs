using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handler for the Monk skill 1 Inch Punch.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Monk_1InchPunch)]
	public class Monk_1InchPunchOverride : IGroundSkillHandler
	{
		private const float SpDepleteRate = 0.25f;
		private const float MnaDamagePerPoint = 100f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var targetHandle = target?.Handle ?? 0;
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 40, width: 25, angle: 10f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			var hitDelay = 450;
			var aniTime = 250;
			var hits = new List<SkillHitInfo>();

			await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, hits, modifySkillHitResult: (sk, attacker, target, result) =>
			{
				if (result.Damage <= 0)
					return result;

				if (target is Character character)
				{
					var currentSp = character.Properties.GetFloat(PropertyName.SP);
					var spDrain = (int)Math.Floor(currentSp * SpDepleteRate);

					if (spDrain > 0)
					{
						character.ModifySp(-spDrain);
						result.Damage += spDrain;
					}
				}
				else if (target is Mob mob)
				{
					var mna = mob.Properties.GetFloat(PropertyName.MNA);
					var bonusDamage = mna * MnaDamagePerPoint;

					if (bonusDamage > 0)
						result.Damage += bonusDamage;
				}

				return result;
			});

			await skill.Wait(TimeSpan.FromMilliseconds(250));

			var debuffDuration = 15000 + skill.Level * 1000;
			SkillResultTargetBuff(caster, skill, BuffId.OneInchPunch_Debuff, skill.Level, 0f, debuffDuration, 1, 100, -1, hits);

			if (caster.IsAbilityActive(AbilityId.Monk6))
				SkillResultTargetBuff(caster, skill, BuffId.Common_Silence, skill.Level, 0f, 5000, 1, 100, -1, hits);
		}
	}
}
