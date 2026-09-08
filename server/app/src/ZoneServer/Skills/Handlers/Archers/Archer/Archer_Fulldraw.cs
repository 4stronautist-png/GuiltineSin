using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Zone.Buffs.Handlers.Common;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Shared.Util.TaskHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Archer
{
	/// <summary>
	/// Handler for the Archer skill Multishot.
	/// </summary>
	[SkillHandler(SkillId.Archer_Fulldraw)]
	public class Archer_Fulldraw : ITargetSkillHandler, IDynamicCasted
	{
		/// <summary>
		/// Called when the user starts casting the skill.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			Send.ZC_PLAY_SOUND_Gendered(caster, "voice_war_atk_long_cast", "voice_atk_long_cast_f");
		}

		/// <summary>
		/// Called when the user stops casting the skill.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			Send.ZC_STOP_SOUND_Gendered(caster, "voice_war_atk_long_cast", "voice_atk_long_cast_f");
		}

		/// <summary>
		/// Handles skill, damaging targets.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		public void Handle(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			var originPos = caster.Position;
			var farPos = target.Position;

			skill.IncreaseOverheat();
			caster.TurnTowards(farPos);
			caster.SetAttackState(true);

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 150, width: 10, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

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
			var hitDelay = TimeSpan.FromMilliseconds(100);
			var aniTime = TimeSpan.FromMilliseconds(100);
			var skillHitDelay = TimeSpan.Zero;

			await skill.Wait(hitDelay);

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var skillHits = new List<SkillHitInfo>();

			var hitTargets = targets.LimitBySDR(caster, skill);

			foreach (var target in hitTargets)
			{
				var skillHitResult = SCR_SkillHit(caster, target, skill);
				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);

				skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, target, skill);
				skillHit.ApplyKnockBack(target);

				skillHits.Add(skillHit);

				var holdDuration = 5 + skill.Level;

				target.StartBuff(BuffId.Hold, skill.Level, 0, TimeSpan.FromSeconds(holdDuration), caster);
			}

			// Apply Link if more than 1 target was hit
			if (hitTargets.Count() > 1)
			{
				var linkDuration = TimeSpan.FromSeconds(5 + skill.Level);
				Link.Apply(caster, hitTargets, linkDuration);
			}

			Send.ZC_SKILL_HIT_INFO(caster, skillHits);
		}
	}
}
