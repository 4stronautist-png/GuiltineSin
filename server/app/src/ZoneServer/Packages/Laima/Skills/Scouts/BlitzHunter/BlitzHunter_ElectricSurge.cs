using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Network.Helpers;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Geometry;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_ElectricSurge_Scout)]
	public class BlitzHunter_ElectricSurgeOverride : BlitzHunter_ElectricSurgeBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_ElectricSurge_Archer)]
	public class BlitzHunter_ElectricSurgeArcherOverride : BlitzHunter_ElectricSurgeBase
	{
	}

	public class BlitzHunter_ElectricSurgeBase : IForceSkillHandler
	{
		private const string CastAnimation = "SKL_ELECTRICSURGE";
		private const string NormalCastSound = "skl_eff_blitzhunter_electricsurge_cast";
		private const string BlitzkriegCastSound = "skl_eff_blitzhunter_electricsurge_blitzkrieg_cast";
		private const string NormalShotSound = "skl_eff_blitzhunter_electricsurge_shot";
		private const string BlitzkriegShotSound = "skl_eff_blitzhunter_electricsurge_blitzkrieg_shot";
		private const string NormalHitSound = "skl_eff_blitzhunter_electricsurge_hit";
		private const string BlitzkriegHitSound = "skl_eff_blitzhunter_electricsurge_blitzkrieg_hit";
		private const string ImpactEffect = "Groundimpact_ElectricSphere_Blue_01";
		private const string ArkTerminalPadEffect = "BlitzHunter_ElectricSurge_Pad";
		private const string ArkTerminalThunderEffect = "eff_pc_elementalist_elementalstripe_thunder";
		private const float ArkTerminalPadHeightOffset = 0.15f;
		private const float ArkTerminalThunderScale = 1.3f;
		private const float ArkTerminalThunderDuration = 1f;
		private static readonly short[] ArkTerminalThunderS1Sequence = [unchecked((short)0xE7B8), 0x1618, 0x0F18, 0x4328, 0x50B8];
		private const float ArkTerminalThunderHeight = -10f;
		private const float ArkTerminalPadNumArg2 = 62.008064f;
		private const float ArkTerminalPadNumArg3 = 100f;
		private const int NormalMaxTargets = 15;
		private const int TerminalMaxTargets = 10;
		// Client skill data uses a circular impact frame with Width="50",
		// which matches the visual area much better than the DB splashRange.
		private const float ArkTerminalPulseRadius = 50f;
		private static readonly TimeSpan ArkTerminalTickInterval = TimeSpan.FromSeconds(1);
		private const int ArkTerminalTickCount = 5;
		private static readonly TimeSpan ImpactSyncDuration = TimeSpan.FromMilliseconds(1200);
		private static readonly TimeSpan ImpactDamageDelay = TimeSpan.FromMilliseconds(180);
		private static readonly TimeSpan PostImpactCleanupDelay = TimeSpan.FromMilliseconds(250);
		private const float ImpactJumpHeight = 0f;
		private const float ImpactJumpAngle = 1f;
		private const float ImpactJumpTime = 0.2f;
		private const float ImpactJumpEase = 1f;
		private const float ImpactSyncStart = 1f;
		private const float ImpactSyncEnd = 0f;
		private const float ImpactEffectSyncEnd = 0.1f;
		private const string SuppressDeathSkillCancelVar = "GuiltineSin.SuppressDeathSkillCancel";

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BlitzHunterSkillHelper.ValidateForceTarget(skill, caster, target))
				return;

			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			skill.IncreaseOverheat();
			caster.TurnTowards(target);
			caster.SetAttackState(true);
			BlitzHunterSkillHelper.ApplySkillAnimationMovementLock(caster, skill, 100);
			Send.ZC_PC_ATKSTATE(caster, true, includeEntity: true);
			BlitzHunterSkillHelper.TryPlayAnimation(caster, CastAnimation);

			BlitzHunterSkillHelper.SendForceTargetSkillReady(caster, skill, target);
			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill);
			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var shotDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 790, 195);
			BlitzHunterSkillHelper.TryPlaySound(caster, isBlitzkrieg ? BlitzkriegCastSound : NormalCastSound);
			skill.Run(caster.IsAbilityActive(AbilityId.BlitzHunter8) && !isBlitzkrieg
				? this.RunArkTerminal(skill, caster, target)
				: this.Explode(skill, caster, target));
		}

		private async Task Explode(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var shotDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 790, 195);
			var hitDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 800, 300);

			if (shotDelay > TimeSpan.Zero)
				await skill.Wait(shotDelay);

			BlitzHunterSkillHelper.TryPlaySound(caster, isBlitzkrieg ? BlitzkriegShotSound : NormalShotSound);

			var beforeHit = hitDelay - shotDelay;
			if (beforeHit > TimeSpan.Zero)
				await skill.Wait(beforeHit);

			var targetPosition = target?.Position ?? caster.Position;
			var targets = BlitzHunterSkillHelper.GetCircleTargets(caster, skill, targetPosition, skill.Data.SplashRange, NormalMaxTargets);
			var multiHitCount = skill.Properties.MultiHitCount > 0 ? skill.Properties.MultiHitCount : 5;
			var hits = this.PrepareDirectHits(caster, skill, targets, SkillModifier.MultiHit(multiHitCount), BlitzHunterSkillHelper.GetScaledDisplayDelay(skill, 50), multiHitCount, 0.1f);

			this.SendImpactSequence(skill, caster, target, targetPosition, hits, isBlitzkrieg ? BlitzkriegHitSound : NormalHitSound);
			if (hits.Count > 0)
				await skill.Wait(ImpactDamageDelay);
			if (hits.Count > 0)
				this.ApplyPreparedHits(caster, hits);

			if (BlitzHunterSkillHelper.HasAccurateHit(hits))
				BlitzHunterSkillHelper.ApplyVolticCatharsisImpact(caster, skill);

			await skill.Wait(PostImpactCleanupDelay);
			await BlitzHunterSkillHelper.FinishCastAsync(skill, caster, hitDelay);
		}

		private async Task RunArkTerminal(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			var hitsAppliedVoltic = false;
			var arkTerminalSkill = new Skill(caster, SkillId.BlitzHunter_ElectricSurge_Abil, 1);
			var shotDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 790, 195);
			var firstHitDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 800, 300);
			var hitAniTime = BlitzHunterSkillHelper.GetScaledDisplayDelay(skill, 50);
			var pulseHandle = (int)BlitzHunterSkillHelper.CreateRetailVisualPadHandle(25000);
			var padShown = false;
			var pulseCenter = Position.Zero;

			try
			{
				if (shotDelay > TimeSpan.Zero)
					await skill.Wait(shotDelay);
				BlitzHunterSkillHelper.TryPlaySound(caster, NormalShotSound);

				var beforeFirstHit = firstHitDelay - shotDelay;
				if (beforeFirstHit > TimeSpan.Zero)
					await skill.Wait(beforeFirstHit);
				var elapsed = firstHitDelay;
				var targetPosition = target?.Position ?? caster.Position;

				// Ark Terminal is layered on top of Electric Surge's normal hit.
				var initialTargets = BlitzHunterSkillHelper.GetCircleTargets(caster, skill, targetPosition, skill.Data.SplashRange, NormalMaxTargets);
				var initialMultiHitCount = skill.Properties.MultiHitCount > 0 ? skill.Properties.MultiHitCount : 5;
				var initialHits = this.PrepareDirectHits(caster, skill, initialTargets, SkillModifier.MultiHit(initialMultiHitCount), hitAniTime, initialMultiHitCount, 0.1f);

				this.SendImpactSequence(skill, caster, target, targetPosition, initialHits, NormalHitSound);
				if (initialHits.Count > 0)
					await skill.Wait(ImpactDamageDelay);
				if (initialHits.Count > 0)
					this.ApplyPreparedHits(caster, initialHits);

				if (BlitzHunterSkillHelper.HasAccurateHit(initialHits))
				{
					BlitzHunterSkillHelper.ApplyVolticCatharsisImpact(caster, skill);
					hitsAppliedVoltic = true;
				}

				pulseCenter = this.GetArkTerminalPulseCenter(initialHits, targetPosition);
				this.SendArkTerminalPadState(skill, caster, pulseCenter, pulseHandle, true);
				padShown = true;

				for (var i = 0; i < ArkTerminalTickCount; i++)
				{
					if (caster.IsDead)
						return;

					await Task.Delay(ArkTerminalTickInterval);
					elapsed += ArkTerminalTickInterval;

					var targets = this.GetArkTerminalTargets(caster, pulseCenter, ArkTerminalPulseRadius, TerminalMaxTargets, arkTerminalSkill);
					var hits = this.PrepareDirectHits(caster, arkTerminalSkill, targets, this.CreateArkTerminalModifier(skill, arkTerminalSkill), hitAniTime, 1, 0.1f);

					this.SendArkTerminalPulseSequence(arkTerminalSkill, caster, pulseCenter, hits, NormalHitSound, i);
					if (hits.Count > 0)
						await skill.Wait(ImpactDamageDelay);
					if (hits.Count > 0)
						this.ApplyPreparedHits(caster, hits);

					if (!hitsAppliedVoltic && BlitzHunterSkillHelper.HasAccurateHit(hits))
					{
						BlitzHunterSkillHelper.ApplyVolticCatharsisImpact(caster, skill);
						hitsAppliedVoltic = true;
					}
				}

				await skill.Wait(PostImpactCleanupDelay);
				await BlitzHunterSkillHelper.FinishCastAsync(skill, caster, elapsed + PostImpactCleanupDelay);
			}
			finally
			{
				if (padShown)
					this.SendArkTerminalPadState(skill, caster, pulseCenter, pulseHandle, false);
			}
		}

		private void SendImpactSequence(Skill skill, ICombatEntity caster, ICombatEntity jumpTarget, Position targetPosition, System.Collections.Generic.List<HitInfo> hits, string hitSound)
		{
			var primingSyncKey = caster.GenerateSyncKey();
			var jumpSyncKey = caster.GenerateSyncKey();
			var effectSyncKey = caster.GenerateSyncKey();
			var impactActor = (IActor)(jumpTarget ?? caster);

			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, primingSyncKey, ImpactSyncDuration);
			Send.ZC_SYNC_END(caster, primingSyncKey, ImpactSyncEnd);
			Send.ZC_SYNC_START(caster, primingSyncKey, ImpactSyncStart);

			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, jumpSyncKey, ImpactSyncDuration);
			Send.ZC_SYNC_END(caster, jumpSyncKey, ImpactSyncEnd);

			if (jumpTarget != null)
				Send.ZC_NORMAL.LeapJump(jumpTarget, targetPosition, ImpactJumpHeight, ImpactJumpAngle, ImpactJumpTime, ImpactJumpEase, ImpactJumpTime, ImpactJumpEase);
			Send.ZC_SYNC_START(caster, jumpSyncKey, ImpactSyncStart);

			foreach (var hit in hits)
				Send.ZC_HIT_INFO(caster, hit.Target, hit);
			Send.ZC_SKILL_HIT_INFO(caster, this.CreateSkillHitInfos(skill, hits));

			Send.ZC_UNITY_GROUND_EFFECT(impactActor, ImpactEffect.GetStringId(), 0.8f, targetPosition, 0f, 0f, 0f, new Direction(2f, 0.1f));
			Send.ZC_SYNC_END(caster, effectSyncKey, ImpactEffectSyncEnd);
			BlitzHunterSkillHelper.TryPlaySound(caster, hitSound);
			if (jumpTarget != null)
				BlitzHunterSkillHelper.TryPlaySound(jumpTarget, hitSound);
			Send.ZC_SYNC_START(caster, effectSyncKey, ImpactSyncStart);
		}

		private void SendArkTerminalPadState(Skill visualSkill, ICombatEntity caster, Position targetPosition, int pulseHandle, bool isVisible)
		{
			var pulseDirection = Direction.East;
			var visualPosition = new Position(targetPosition.X, ArkTerminalPadHeightOffset, targetPosition.Z);

			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, ArkTerminalPadEffect, visualSkill, visualPosition, pulseDirection, 0f, ArkTerminalPadNumArg2, pulseHandle, ArkTerminalPadNumArg3, isVisible: isVisible, visualSkillLevel: 2);
		}

		private void SendArkTerminalPulseSequence(Skill damageSkill, ICombatEntity caster, Position targetPosition, System.Collections.Generic.List<HitInfo> hits, string hitSound, int tickIndex)
		{
			var primingSyncKey = caster.GenerateSyncKey();
			var effectSyncKey = caster.GenerateSyncKey();
			var thunderPosition = new Position(targetPosition.X, ArkTerminalThunderHeight, targetPosition.Z);
			var thunderS1 = ArkTerminalThunderS1Sequence[tickIndex % ArkTerminalThunderS1Sequence.Length];

			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, primingSyncKey, ImpactSyncDuration);
			Send.ZC_SYNC_END(caster, primingSyncKey, ImpactSyncEnd);
			Send.ZC_SYNC_START(caster, primingSyncKey, ImpactSyncStart);

			Send.ZC_GROUND_EFFECT(caster, thunderPosition, ArkTerminalThunderEffect, ArkTerminalThunderScale, ArkTerminalThunderDuration, 0f, 0f, thunderS1);

			foreach (var hit in hits)
				Send.ZC_HIT_INFO(caster, hit.Target, hit);
			Send.ZC_SKILL_HIT_INFO(caster, this.CreateSkillHitInfos(damageSkill, hits));

			Send.ZC_SYNC_END(caster, effectSyncKey, ImpactEffectSyncEnd);
			BlitzHunterSkillHelper.TryPlaySound(caster, hitSound);
			Send.ZC_SYNC_START(caster, effectSyncKey, ImpactSyncStart);
		}

		private System.Collections.Generic.List<SkillHitInfo> CreateSkillHitInfos(Skill skill, System.Collections.Generic.List<HitInfo> hits)
		{
			var result = new System.Collections.Generic.List<SkillHitInfo>(hits.Count);

			foreach (var hit in hits)
			{
				var skillHitResult = new SkillHitResult
				{
					Damage = hit.Damage,
					Result = hit.ResultType,
					HitCount = hit.HitCount,
				};

				var skillHitInfo = new SkillHitInfo(hit.Attacker, hit.Target, skill, skillHitResult, hit)
				{
					AniTime = hit.AniTime,
					ForceId = hit.ForceId,
				};

				result.Add(skillHitInfo);
			}

			return result;
		}

		private System.Collections.Generic.List<HitInfo> PrepareDirectHits(ICombatEntity caster, Skill skill, System.Collections.Generic.IEnumerable<ICombatEntity> targets, SkillModifier modifier, TimeSpan aniTime, int hitCountOverride, float unkFloat2)
		{
			var hits = new System.Collections.Generic.List<HitInfo>();

			foreach (var target in targets)
			{
				if (target == null || target.IsDead)
					continue;

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				var hit = new HitInfo(caster, target, skill, skillHitResult, aniTime)
				{
					UnkFloat2 = unkFloat2,
					HitCount = hitCountOverride,
				};

				hits.Add(hit);
			}

			return hits;
		}

		private SkillModifier CreateArkTerminalModifier(Skill electricSurgeSkill, Skill arkTerminalSkill)
		{
			var targetFactor = electricSurgeSkill.SkillFactor * 0.2f;

			return new SkillModifier
			{
				// Ark Terminal pulses use 20% of Electric Surge's factor,
				// but still identify as the Ark Terminal ability skill.
				SkillFactorBonus = targetFactor - arkTerminalSkill.SkillFactor,
				HitCount = 1,
			};
		}

		private Position GetArkTerminalPulseCenter(System.Collections.Generic.List<HitInfo> initialHits, Position fallbackPosition)
		{
			if (initialHits.Count > 0)
			{
				var primaryTarget = initialHits[0].Target;
				if (primaryTarget != null)
					return new Position(primaryTarget.Position.X, 0f, primaryTarget.Position.Z);
			}

			return new Position(fallbackPosition.X, 0f, fallbackPosition.Z);
		}

		private System.Collections.Generic.IReadOnlyList<ICombatEntity> GetArkTerminalTargets(ICombatEntity caster, Position center, float radius, int maxTargets, Skill skill)
		{
			return caster.Map.GetAttackableEnemiesInPosition(caster, center, radius, skill.Data.HitType)
				.LimitBySDR(caster, skill)
				.Limit(maxTargets)
				.Where(target => target != null && !target.IsDead)
				.Take(maxTargets)
				.ToList();
		}

		private void ApplyPreparedHits(ICombatEntity caster, System.Collections.Generic.IEnumerable<HitInfo> hits)
		{
			var markedTargets = new System.Collections.Generic.HashSet<ICombatEntity>();

			foreach (var hit in hits)
			{
				hit.Target.SetTempVar(SuppressDeathSkillCancelVar, 1f);
				markedTargets.Add(hit.Target);
			}

			foreach (var hit in hits)
				hit.Target.TakeDamage(hit.Damage, caster);

			foreach (var target in markedTargets)
			{
				if (!target.IsDead)
					target.RemoveTempVar(SuppressDeathSkillCancelVar);
			}
		}
	}
}
