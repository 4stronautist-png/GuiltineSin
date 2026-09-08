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
using GuiltineSin.Zone.Abilities.Handlers.Swordsmen.Rodelero;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Rodelero
{
	/// <summary>
	/// Handler for the Rodelero skill Slithering.
	/// Channeled evasive maneuver that grants block bonuses and causes
	/// enemy attacks to miss. Final hit knocks down and slows enemies.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Rodelero_Slithering)]
	public class Rodelero_SlitheringOverride : IGroundSkillHandler, IDynamicCasted
	{
		/// <summary>
		/// Called when the skill begins channeling.
		/// </summary>
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			caster.RemoveBuff(BuffId.Slithering_Buff);
			caster.StartBuff(BuffId.Slithering_Buff, skill.Level, 0f, TimeSpan.Zero, caster);
			caster.PlaySound("voice_archer_camouflage_shot", "voice_archer_m_camouflage_shot");
		}

		/// <summary>
		/// Called when the skill channeling ends.
		/// </summary>
		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.RemoveBuff(BuffId.Slithering_Buff);
			caster.StopSound("voice_archer_camouflage_shot", "voice_archer_m_camouflage_shot");
		}

		/// <summary>
		/// Handles the Slithering skill execution.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();
			Send.ZC_SKILL_READY(caster, skill, skillHandle, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.Attack(skill, caster, originPos, farPos));
		}

		/// <summary>
		/// Executes the multi-hit attack.
		/// </summary>
		private async Task Attack(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 50, width: 40);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			var aniTime = TimeSpan.FromMilliseconds(300);
			var skillHitDelay = TimeSpan.Zero;
			await skill.Wait(TimeSpan.FromMilliseconds(150));

			var hits = new List<SkillHitInfo>();
			var targetList = caster.Map.GetAttackableEnemiesIn(caster, splashArea);

			foreach (var target in targetList.LimitBySDR(caster, skill))
			{
				var modifier = SkillModifier.Default;
				modifier.BonusPAtk = Rodelero31.GetBonusPAtk(caster);
				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				
				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);
				skillHit.HitEffect = HitEffect.Impact;

				if (skillHitResult.Damage > 0)
				{
					if (target.IsKnockdownable())
					{
						skillHit.KnockBackInfo = new KnockBackInfo(caster.Position, target, KnockBackType.KnockDown, 120, 90);
						skillHit.HitInfo.KnockBackType = KnockBackType.KnockDown;
						target.ApplyKnockdown(caster, skill, skillHit);
					}

					target.StartBuff(BuffId.Common_Slow, 1, 0f, TimeSpan.FromMilliseconds(5000), caster);
				}

				hits.Add(skillHit);
			}

			Send.ZC_SKILL_HIT_INFO(caster, hits);
		}
	}
}
