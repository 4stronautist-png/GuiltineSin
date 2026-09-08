using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_VoltChain_Scout)]
	public class BlitzHunter_VoltChainOverride : BlitzHunter_VoltChainBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_VoltChain_Archer)]
	public class BlitzHunter_VoltChainArcherOverride : BlitzHunter_VoltChainBase
	{
	}

	public class BlitzHunter_VoltChainBase : IForceSkillHandler
	{
		private const string CastAnimation = "SKL_VOLTCHAIN";
		private const float SpawnDistance = 10f;
		private const float BounceRange = 80f;
		private const float OrbSeekRange = 600f;
		private const int TickCount = 20;
		private const int MaxTargets = 10;
		private const float ProjectileSpeed = 80f;
		private const float OrbHeight = 30f;
		private const ushort InteractionInfoType = 0x05EF;
		private const ushort InteractionInfoUnk = 0x886E;
		private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(350);
		private static readonly TimeSpan OrbSearchInterval = TimeSpan.FromMilliseconds(100);
		private static readonly TimeSpan BounceStepDelay = TimeSpan.FromMilliseconds(110);
		private const float VisualTravelScale = 1.45f;
		private static readonly TimeSpan MinTravelDelay = TimeSpan.FromMilliseconds(280);
		private static readonly TimeSpan MaxTravelDelay = TimeSpan.FromMilliseconds(3200);
		private static readonly TimeSpan ImpactBufferDelay = TimeSpan.FromMilliseconds(380);
		private static readonly TimeSpan RangeDebugInterval = TimeSpan.FromMilliseconds(35);
		private const float RangeDebugF1 = 0.01f;
		private const float RangeDebugF2 = 0f;
		private const float RangeDebugF3 = 30f;
		private const float RangeDebugF4 = 0f;
		private const float RangeDebugF5 = -1f;
		private const float RangeDebugF6 = 0f;
		private const string NormalCastSound = "skl_eff_blitzhunter_voltchain_shot";
		private const string BlitzkriegCastSound = "skl_eff_blitzhunter_voltchain_blitzkrieg_shot";
		private static readonly byte[][] RetailNoticePayloads =
		[
			[0x01, 0xE8, 0x2D, 0x02, 0x07, 0x01, 0x00, 0x10, 0x01, 0x18, 0x2C, 0x00, 0x00],
			[0x01, 0x6D, 0xEE, 0xF5, 0x15, 0x01, 0x00, 0x10, 0x01, 0xE9, 0x03, 0x00, 0x00],
			[0x01, 0xE7, 0x9B, 0x32, 0x08, 0x01, 0x00, 0x10, 0x01, 0xD5, 0x03, 0x00, 0x00],
			[0x01, 0x5E, 0x3B, 0xF4, 0x35, 0x01, 0x00, 0x10, 0x01, 0xD5, 0x03, 0x00, 0x00],
			[0x01, 0x84, 0x20, 0x4E, 0x05, 0x01, 0x00, 0x10, 0x01, 0x1F, 0x05, 0x00, 0x00],
			[0x01, 0x30, 0x8F, 0x03, 0x1A, 0x01, 0x00, 0x10, 0x01, 0x47, 0x05, 0x00, 0x00],
			[0x01, 0x85, 0x23, 0xF9, 0x14, 0x01, 0x00, 0x10, 0x01, 0xE9, 0x03, 0x00, 0x00],
			[0x01, 0xD4, 0xD5, 0x20, 0x09, 0x01, 0x00, 0x10, 0x01, 0x1F, 0x05, 0x00, 0x00],
		];
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var spMultiplier = caster.IsAbilityActive(AbilityId.BlitzHunter10) ? 1.5f : 1f;
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
				return;

			skill.IncreaseOverheat();
			if (target != null && !target.IsDead)
				caster.TurnTowards(target);
			caster.SetAttackState(true);

			BlitzHunterSkillHelper.SendForceTargetSkillReady(caster, skill, target);
			Send.ZC_SKILL_FORCE_TARGET(caster, target, skill);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, target?.Handle ?? 0, caster.Position, caster.Direction, Position.Zero);
			BlitzHunterSkillHelper.TryPlayAnimation(caster, CastAnimation);
			skill.Run(this.Attack(skill, caster, target, BlitzHunterSkillHelper.CreateRetailVisualPadHandle()));
		}

		private async Task Attack(Skill skill, ICombatEntity caster, ICombatEntity initialTarget, int orbEffectHandle)
		{
			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var hasInitialTarget = initialTarget != null && !initialTarget.IsDead;
			var setupDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 700, 450);
			BlitzHunterSkillHelper.TryPlaySound(caster, isBlitzkrieg ? BlitzkriegCastSound : NormalCastSound);
			if (setupDelay > TimeSpan.Zero)
				await skill.Wait(setupDelay);

			if (caster.IsDead)
				return;

			var relativeSpawnPosition = caster.Position.GetRelative(caster.Direction, SpawnDistance);
			var spawnPosition = new Position(relativeSpawnPosition.X, 0f, relativeSpawnPosition.Z);
			var orbPosition = spawnPosition;
			var orbSpawned = false;
			var orbHidden = false;
			var animationCleanedUp = false;

			try
			{
				Send.ZC_InteractionInfo(caster, InteractionInfoType, InteractionInfoUnk, SpawnDistance);
				Send.ZC_NORMAL.Unknown_17C_BlitzNotice(caster, CreateRetailNoticePayload(0));
				Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, "BlitzHunter_VoltChain_Pad", skill, spawnPosition, caster.Direction, 0f, 10f, orbEffectHandle, OrbHeight, visualSkillLevel: skill.Level);
				orbSpawned = true;
				this.SendRetailControlPackets(caster, skill);
				var elapsed = setupDelay;
				var hitsAppliedVoltic = false;
				var bossDamageBonusOnly = caster.IsAbilityActive(AbilityId.BlitzHunter10);
				var movementStarted = false;
				var currentTarget = initialTarget;
				var successfulTargetIndex = 0;
				var remainingLifetime = TimeSpan.FromTicks(TickInterval.Ticks * TickCount);

				while (remainingLifetime > TimeSpan.Zero && successfulTargetIndex < TickCount)
				{
					if (caster.IsDead)
						return;

					currentTarget = this.FindNearestVoltChainTarget(caster, orbPosition, currentTarget);
					if (currentTarget == null)
					{
						var searchDelay = remainingLifetime < OrbSearchInterval ? remainingLifetime : OrbSearchInterval;
						if (searchDelay <= TimeSpan.Zero)
							break;

						await skill.Wait(searchDelay);
						elapsed += searchDelay;
						remainingLifetime -= searchDelay;
						continue;
					}

					if (!currentTarget.IsDead)
					{
						var destination = currentTarget.Position;
						var travelDelay = GetProjectileTravelDelay(orbPosition, destination);

						if (!movementStarted)
						{
							var orbSyncKey = ZoneServer.Instance.World.CreateSkillHandle();
							Send.ZC_SYNC_START(caster, orbSyncKey, 1);
							Send.ZC_NORMAL.SkillEffectMovement(caster, orbEffectHandle, destination, ProjectileSpeed);
							Send.ZC_SYNC_END(caster, orbSyncKey, 0);
							Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, orbSyncKey, travelDelay);
							movementStarted = true;
						}
						else if (successfulTargetIndex > 0)
						{
							Send.ZC_NORMAL.Unknown_17C_BlitzNotice(caster, CreateRetailNoticePayload(successfulTargetIndex));
						}

						var trackedDestination = await this.TrackOrbMovementToTarget(skill, caster, orbEffectHandle, orbPosition, currentTarget, travelDelay);
						await this.WaitForImpactAlignment(skill, trackedDestination, currentTarget, travelDelay);
						orbPosition = trackedDestination;
						elapsed += travelDelay;
						remainingLifetime -= travelDelay;
						elapsed += this.GetImpactAlignmentDelay(trackedDestination, currentTarget, travelDelay);
						remainingLifetime -= this.GetImpactAlignmentDelay(trackedDestination, currentTarget, travelDelay);

						this.ApplyHit(caster, skill, currentTarget, bossDamageBonusOnly, ref hitsAppliedVoltic);
						successfulTargetIndex++;
					}

					if (successfulTargetIndex < TickCount)
					{
						var cadenceDelay = remainingLifetime < TickInterval ? remainingLifetime : TickInterval;
						if (cadenceDelay > TimeSpan.Zero)
						{
							await skill.Wait(cadenceDelay);
							elapsed += cadenceDelay;
							remainingLifetime -= cadenceDelay;
						}
					}
				}

				this.HideOrbEffect(caster, skill, orbEffectHandle, orbPosition);
				orbHidden = true;
				this.CleanupCastAnimation(caster);
				animationCleanedUp = true;
			}
			finally
			{
				if (orbSpawned && !orbHidden)
					this.HideOrbEffect(caster, skill, orbEffectHandle, orbPosition);

				if (!animationCleanedUp)
					this.CleanupCastAnimation(caster);
			}
		}

		private async Task EmitRangeDebugBurst(Skill skill, ICombatEntity caster, Position fromPosition, Position destination, TimeSpan duration)
		{
			var stepCount = Math.Max(1, (int)Math.Ceiling(duration.TotalMilliseconds / RangeDebugInterval.TotalMilliseconds));
			var waited = TimeSpan.Zero;
			for (var step = 0; step < stepCount; step++)
			{
				var t = (stepCount == 1 ? 1f : (step + 1f) / stepCount);
				var currentPosition = Position.Lerp(fromPosition, destination, t);
				var direction = fromPosition.GetDirection(currentPosition);
				if (direction == Direction.Zero)
					direction = caster.Direction;

				Send.ZC_SKILL_RANGE_DBG(caster, currentPosition, direction, destination, RangeDebugF1, RangeDebugF2, RangeDebugF3, RangeDebugF4, RangeDebugF5, RangeDebugF6, includeCaster: true);

				var remaining = duration - waited;
				if (remaining <= TimeSpan.Zero)
					continue;

				var stepDelay = (step < stepCount - 1 ? TimeSpan.FromMilliseconds(Math.Min(RangeDebugInterval.TotalMilliseconds, remaining.TotalMilliseconds)) : remaining);
				if (stepDelay > TimeSpan.Zero)
				{
					await skill.Wait(stepDelay);
					waited += stepDelay;
				}
			}
		}

		private async Task<Position> TrackOrbMovementToTarget(Skill skill, ICombatEntity caster, int orbEffectHandle, Position fromPosition, ICombatEntity target, TimeSpan duration)
		{
			var waited = TimeSpan.Zero;
			var currentFrom = fromPosition;
			var latestDestination = target?.Position ?? fromPosition;

			while (waited < duration)
			{
				if (target != null && !target.IsDead)
					latestDestination = target.Position;

				Send.ZC_NORMAL.SkillEffectMovement(caster, orbEffectHandle, latestDestination, ProjectileSpeed);

				var stepDelay = TimeSpan.FromMilliseconds(Math.Min(OrbSearchInterval.TotalMilliseconds, (duration - waited).TotalMilliseconds));
				await this.EmitRangeDebugBurst(skill, caster, currentFrom, latestDestination, stepDelay);
				waited += stepDelay;
				currentFrom = latestDestination;
			}

			return latestDestination;
		}

		private static TimeSpan GetProjectileTravelDelay(Position fromPosition, Position destination)
		{
			var distance = (float)fromPosition.Get2DDistance(destination);
			// Retail's moving pad reads visually slower than a plain distance/speed estimate,
			// so we scale the travel window to delay the server hit until the orb appears to collide.
			var scaledMilliseconds = distance / ProjectileSpeed * 1000f * VisualTravelScale;
			var clampedMilliseconds = Math.Clamp(scaledMilliseconds, (float)MinTravelDelay.TotalMilliseconds, (float)MaxTravelDelay.TotalMilliseconds);
			return TimeSpan.FromMilliseconds(Math.Max((float)BounceStepDelay.TotalMilliseconds, clampedMilliseconds));
		}

		private async Task WaitForImpactAlignment(Skill skill, Position sentDestination, ICombatEntity target, TimeSpan sentTravelDelay)
		{
			var alignmentDelay = this.GetImpactAlignmentDelay(sentDestination, target, sentTravelDelay);
			if (alignmentDelay > TimeSpan.Zero)
				await skill.Wait(alignmentDelay);
		}

		private TimeSpan GetImpactAlignmentDelay(Position sentDestination, ICombatEntity target, TimeSpan sentTravelDelay)
		{
			var delay = ImpactBufferDelay;
			if (target == null || target.IsDead)
				return delay;

			var updatedDestination = target.Position;
			var driftDistance = (float)sentDestination.Get2DDistance(updatedDestination);
			if (driftDistance <= 0f)
				return delay;

			var adjustedTravelDelay = GetProjectileTravelDelay(sentDestination, updatedDestination);
			var extraDelay = adjustedTravelDelay - sentTravelDelay;
			if (extraDelay > TimeSpan.Zero)
				delay += extraDelay;

			return delay;
		}

		private void HideOrbEffect(ICombatEntity caster, Skill skill, int orbEffectHandle, Position orbPosition)
		{
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, "BlitzHunter_VoltChain_Pad", skill, orbPosition, caster.Direction, 0f, 10f, orbEffectHandle, OrbHeight, isVisible: false, visualSkillLevel: skill.Level);
		}

		private void ApplyHit(ICombatEntity caster, Skill skill, ICombatEntity target, bool onlyBossTarget, ref bool hitsAppliedVoltic)
		{
			if (target == null || target.IsDead)
				return;

			var modifier = SkillModifier.Default;
			if (onlyBossTarget && target.Rank == MonsterRank.Boss)
				modifier.DamageMultiplier *= 1.5f;

			var hits = BlitzHunterSkillHelper.DealDirectHits(caster, skill, new[] { target }, modifier, TimeSpan.Zero, hitCountOverride: 0);
			foreach (var hit in hits)
				Send.ZC_HIT_INFO(caster, hit.Target, hit);

			if (!hitsAppliedVoltic && BlitzHunterSkillHelper.HasAccurateHit(hits))
			{
				BlitzHunterSkillHelper.ApplyVolticCatharsis(caster, skill);
				hitsAppliedVoltic = true;
			}
		}

		private ICombatEntity FindNearestVoltChainTarget(ICombatEntity caster, Position orbPosition, ICombatEntity currentTarget)
		{
			if (this.IsVoltChainTargetCandidate(currentTarget))
				return currentTarget;

			var aroundOrb = caster.Map.GetAttackableEnemiesInPosition(caster, orbPosition, OrbSeekRange)
				.Where(this.IsVoltChainTargetCandidate);

			var aroundCaster = caster.Map.GetAttackableEnemiesInPosition(caster, caster.Position, OrbSeekRange)
				.Where(this.IsVoltChainTargetCandidate)
				.Where(target => !ReferenceEquals(target, currentTarget));

			return aroundOrb
				.Concat(aroundCaster)
				.Distinct()
				.OrderBy(target => target.Position.Get2DDistance(orbPosition))
				.ThenBy(target => target.Position.Get2DDistance(caster.Position))
				.FirstOrDefault();
		}

		private bool IsVoltChainTargetCandidate(ICombatEntity target)
		{
			if (target == null || target.IsDead)
				return false;

			return true;
		}

		private void SendRetailControlPackets(ICombatEntity caster, Skill skill)
		{
			Send.ZC_NORMAL.SkillCancel(caster, skill.Id);
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
		}

		private void CleanupCastAnimation(ICombatEntity caster)
		{
			Send.ZC_NORMAL.StopAnimation(caster);
			Send.ZC_NORMAL.ResetStdAnim(caster);
			caster.SetAttackState(false);
			Send.ZC_PC_ATKSTATE(caster, false, includeEntity: true);
		}

		private static byte[] CreateRetailNoticePayload(int hopIndex)
		{
			var source = RetailNoticePayloads[Math.Abs(hopIndex) % RetailNoticePayloads.Length];
			return (byte[])source.Clone();
		}
	}
}
