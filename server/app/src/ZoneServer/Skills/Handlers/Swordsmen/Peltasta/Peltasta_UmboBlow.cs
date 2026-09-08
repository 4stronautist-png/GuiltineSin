using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Abilities.Handlers.Swordsmen.Peltasta;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Util;
using static GuiltineSin.Shared.Util.TaskHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Peltasta
{
	/// <summary>
	/// Handler for the Peltasta Skill Umbo Blow.
	/// </summary>
	[SkillHandler(SkillId.Peltasta_UmboBlow)]
	public class Peltasta_UmboBlow : IGroundSkillHandler
	{
		private readonly static TimeSpan StunDuration = TimeSpan.FromSeconds(3);
		private const int BuffRemoveChancePerLevel = 8;
		private const float LoopholeDamageMultiplier = 0.5f;
		private const float StunChancePerLevel = 5f;

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

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 25, width: 45, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, null);

			skill.Run(this.Attack(skill, caster, splashArea));
		}

		/// <summary>
		/// Executes the actual attack after a delay.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="splashArea"></param>
		private async Task Attack(Skill skill, ICombatEntity caster, ISplashArea splashArea)
		{
			var hitDelay = TimeSpan.FromMilliseconds(400);
			var aniTime = TimeSpan.FromMilliseconds(50);
			var skillHitDelay = TimeSpan.Zero;

			await skill.Wait(hitDelay);

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var hits = new List<SkillHitInfo>();

			foreach (var target in targets.LimitBySDR(caster, skill))
			{
				var modifier = SkillModifier.Default;
				modifier.BonusPAtk = Peltasta38.GetBonusPAtk(caster);

				// At one point this seemed to instead use a status that was applied
				// by the removed Peltasta7 ability and the bonus was 200%
				if (caster.IsBuffActive(BuffId.Peltasta5_Guard_Buff))
					modifier.DamageMultiplier += LoopholeDamageMultiplier;

				// Increase damage by 10% if target is under the effect of
				// Swashbuckling from the caster
				if (target.TryGetBuff(BuffId.SwashBuckling_Debuff, out var swashBuckingDebuff))
				{
					if (swashBuckingDebuff.Caster == caster)
						modifier.DamageMultiplier += 0.10f;
				}

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);

				if (caster.TryGetActiveAbilityLevel(AbilityId.Peltasta8, out var knockdownLevel))
				{
					// Knockback power is 40 * level
					skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, target, skill);
					skillHit.ApplyKnockBack(target);
				}
				else
				{
					skillHit.HitEffect = HitEffect.Impact;
				}

				hits.Add(skillHit);

				// Note: This ability was repurposed to Rim Blow after Umbo Blow was removed,
				// which is why the description mentions it affects Umbo Blow.
				if (caster.TryGetActiveAbilityLevel(AbilityId.Impact, out var stunLevel) && RandomProvider.Get().Next(100) < stunLevel * StunChancePerLevel)
					target.StartBuff(BuffId.Stun, stunLevel, 0, StunDuration, caster);

				var buffRemoveChance = BuffRemoveChancePerLevel * skill.Level;
				target.RemoveRandomBuff(buffRemoveChance);
			}

			Send.ZC_SKILL_HIT_INFO(caster, hits);
		}
	}
}
