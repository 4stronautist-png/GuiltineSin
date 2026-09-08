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
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handler for the Monk skill Energy Blast.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Monk_EnergyBlast)]
	public class Monk_EnergyBlastOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.PlaySound("voice_cleric_sunrayshand_shot", "voice_cleric_m_sunrayshand_shot");
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.StopSound("voice_cleric_sunrayshand_shot", "voice_cleric_m_sunrayshand_shot");
		}

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

			skill.Run(this.Attack(skill, caster, originPos, farPos));
		}

		private async Task Attack(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var delayBetweenHits = TimeSpan.FromMilliseconds(100);
			var totalHits = 60;

			var goldenBellBonus = 0f;
			if (caster.TryGetBuff(BuffId.Golden_Bell_Shield_Buff, out var gbsBuff))
			{
				var skillLevel = gbsBuff.NumArg1;
				var totalSpConsumed = gbsBuff.Vars.GetFloat("TotalSpConsumed");
				goldenBellBonus = totalSpConsumed * (1.5f + 0.15f * skillLevel);
				caster.StopBuff(BuffId.Golden_Bell_Shield_Buff);
			}

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 280, width: 40, angle: 0f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			await skill.Wait(TimeSpan.FromMilliseconds(200));

			for (var i = 0; i < totalHits; i++)
			{
				if (caster.IsDead)
					break;

				var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);

				var isLastHit = i == totalHits - 1;

				var hits = new List<SkillHitInfo>();

				foreach (var target in targets.Take(15))
				{
					var skillHitResult = SCR_SkillHit(caster, target, skill);
					skillHitResult.Damage += goldenBellBonus;
					target.TakeDamage(skillHitResult.Damage, caster);

					var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, delayBetweenHits, TimeSpan.Zero);

					if (skillHitResult.Damage > 0 && target.IsKnockdownable() && !caster.IsAbilityActive(AbilityId.Monk8))
					{
						skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, skillHit.Target, KnockBackType.KnockBack, 80, 30);
						skillHit.HitInfo.KnockBackType = KnockBackType.KnockBack;
						target.ApplyKnockdown(caster, skill, skillHit);
					}

					hits.Add(skillHit);
				}

				Send.ZC_SKILL_HIT_INFO(caster, hits);

				await skill.Wait(delayBetweenHits);
			}

			Send.ZC_SKILL_DISABLE(caster);
			Send.ZC_NORMAL.SkillCancel(caster, SkillId.Monk_EnergyBlast);
			Send.ZC_NORMAL.SkillCancelCancel(caster, SkillId.Monk_EnergyBlast);
		}
	}
}
