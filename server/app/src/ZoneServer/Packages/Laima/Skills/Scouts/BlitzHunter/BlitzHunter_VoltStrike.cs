using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;
using static Melia.Zone.Skills.SkillUseFunctions;

namespace Melia.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_VoltStrike_Scout)]
	public class BlitzHunter_VoltStrikeOverride : BlitzHunter_VoltStrikeBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_VoltStrike_Archer)]
	public class BlitzHunter_VoltStrikeArcherOverride : BlitzHunter_VoltStrikeBase
	{
	}

	public class BlitzHunter_VoltStrikeBase : IMeleeGroundSkillHandler, IGroundSkillHandler
	{
		private const string CastAnimation = "SKL_BOLTSTRIKE";
		private const int MaxTargets = 10;
		private const float EffectDistance = 40f;
		private const string CastSound = "skl_eff_blitzhunter_voltstrike";
		private const int BuffedRetailPadHandleFloor = 25000;
		private static readonly byte[][] RetailNoticePayloads =
		[
			[0x01, 0x7F, 0xC7, 0x2F, 0x00, 0x01, 0x00, 0x10, 0x01, 0x69, 0x06, 0x00, 0x00],
			[0x01, 0x16, 0x69, 0x87, 0x0C, 0x01, 0x00, 0x10, 0x01, 0x19, 0x2C, 0x00, 0x00],
			[0x01, 0x46, 0xBF, 0xC6, 0x04, 0x01, 0x00, 0x10, 0x01, 0xE9, 0x03, 0x00, 0x00],
		];
		private static readonly TimeSpan RangeDebugInterval = TimeSpan.FromMilliseconds(35);
		private static readonly TimeSpan BlitzkriegRangeDebugInterval = TimeSpan.FromMilliseconds(25);
		private const int BlitzkriegRangeDebugSteps = 12;
		private const float RangeDebugF1 = 0f;
		private const float RangeDebugF2 = 0f;
		private const float RangeDebugF3 = 40f;
		private const float RangeDebugF4 = 0f;
		private const float RangeDebugF5 = -1f;
		private const float RangeDebugF6 = 0f;
		private static readonly TimeSpan EffectCleanupDelay = TimeSpan.FromMilliseconds(200);
		private static readonly TimeSpan NormalStopAnimationLead = TimeSpan.FromMilliseconds(120);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, System.Collections.Generic.IList<ICombatEntity> targets)
			=> this.HandleCore(skill, caster, originPos, farPos, targets != null && targets.Count > 0 ? targets[0] : null);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
			=> this.HandleCore(skill, caster, originPos, farPos, target);

		private void HandleCore(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var direction = BlitzHunterSkillHelper.GetDirection(caster, originPos, farPos);
			var clampedFarPos = caster.Position.GetRelative(direction, skill.Properties.GetFloat(PropertyName.MaxR));

			skill.IncreaseOverheat();
			caster.TurnTowards(direction);
			caster.SetAttackState(true);
			Send.ZC_PC_ATKSTATE(caster, true, includeEntity: true);
			BlitzHunterSkillHelper.TryPlayAnimation(caster, CastAnimation, b1: (byte)(isBlitzkrieg ? 1 : 0));

			var effectHandle = isBlitzkrieg
				? BlitzHunterSkillHelper.CreateRetailVisualPadHandle(BuffedRetailPadHandleFloor)
				: BlitzHunterSkillHelper.CreateRetailVisualPadHandle();
			var groundForceId = ZoneServer.Instance.World.CreateSkillHandle();

			BlitzHunterSkillHelper.SendGroundSkillReady(caster, skill);
			if (isBlitzkrieg)
				Send.ZC_SKILL_MELEE_GROUND_VARIANT(caster, skill, clampedFarPos, 2, groundForceId, null);
			else
				Send.ZC_SKILL_MELEE_GROUND(caster, skill, clampedFarPos, groundForceId, null);
			Send.ZC_NORMAL.UpdateSkillEffectIncludingEntity(caster, isBlitzkrieg ? 1 : 0, target?.Handle ?? 0, clampedFarPos, direction, Position.Zero);
			skill.Run(this.Attack(skill, caster, clampedFarPos, direction, effectHandle, target));
		}

		private async System.Threading.Tasks.Task Attack(Skill skill, ICombatEntity caster, Position effectStartPosition, Direction direction, int effectHandle, ICombatEntity initialTarget)
		{
			var isBlitzkrieg = BlitzHunterSkillHelper.IsBlitzkriegActive(skill);
			var soundDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 500, 100);
			var effectDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 600, 200);
			var hitDelay = BlitzHunterSkillHelper.GetVariantDelay(skill, 800, 500);
			var elapsed = TimeSpan.Zero;

			if (soundDelay > TimeSpan.Zero)
			{
				await skill.Wait(soundDelay);
				elapsed = soundDelay;
			}

			BlitzHunterSkillHelper.TryPlaySound(caster, CastSound);

			var beforeEffect = effectDelay - elapsed;
			if (beforeEffect > TimeSpan.Zero)
			{
				await skill.Wait(beforeEffect);
				elapsed = effectDelay;
			}

			var effectOrigin = new Position(effectStartPosition.X, 0f, effectStartPosition.Z);
			var effectAngle = NormalizeEffectAngle(direction);
			var effectDestination = effectStartPosition.GetRelative(direction, skill.Properties.GetFloat(PropertyName.MaxR));
			var effectSyncKey = ZoneServer.Instance.World.CreateSkillHandle();
			var effectSyncPrimingKey = ZoneServer.Instance.World.CreateSkillHandle();

			Send.ZC_NORMAL.Unknown_17C_BlitzNotice(caster, CreateRetailNoticePayload(isBlitzkrieg ? 2 : 0));
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, "BlitzHunter_VoltStrike_Pad", skill, effectOrigin, direction, effectAngle, 0f, effectHandle, EffectDistance, visualSkillLevel: 2);
			Send.ZC_SYNC_START(caster, effectSyncPrimingKey, 1f);
			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, effectSyncKey, hitDelay);
			Send.ZC_SYNC_END(caster, effectSyncKey, 0f);
			Send.ZC_SYNC_START(caster, effectSyncKey, 1f);

			var beforeHit = hitDelay - elapsed;
			if (beforeHit > TimeSpan.Zero)
			{
				await this.EmitRangeDebugBurst(skill, caster, effectOrigin, effectDestination, direction, beforeHit, isBlitzkrieg);
				elapsed = hitDelay;
			}

			elapsed = hitDelay;
			Send.ZC_NORMAL.SkillEffectMovement(caster, effectHandle, effectDestination, 1000f, 200f);

			var skillTargets = this.GetVoltStrikeTargets(caster, skill, direction, initialTarget);
			var multiHitCount = skill.Properties.MultiHitCount > 0 ? skill.Properties.MultiHitCount : 3;
			var hits = isBlitzkrieg
				? this.PrepareDirectHits(caster, skill, skillTargets, SkillModifier.MultiHit(multiHitCount), BlitzHunterSkillHelper.GetScaledDisplayDelay(skill, 50), multiHitCount)
				: BlitzHunterSkillHelper.DealDirectHits(caster, skill, skillTargets, SkillModifier.MultiHit(multiHitCount), BlitzHunterSkillHelper.GetScaledDisplayDelay(skill, 50), hitCountOverride: multiHitCount);

			if (hits.Count > 0 && isBlitzkrieg)
			{
				await this.EmitRepeatedHitInfo(skill, caster, hits);
				this.ApplyPreparedHits(caster, hits);
			}
			else if (hits.Count > 0)
			{
				foreach (var hit in hits)
					Send.ZC_HIT_INFO(caster, hit.Target, hit);
			}

			if (hits.Count > 0 && BlitzHunterSkillHelper.HasAccurateHit(hits))
				BlitzHunterSkillHelper.ApplyVolticCatharsis(caster, skill);

			await skill.Wait(EffectCleanupDelay);
			elapsed += EffectCleanupDelay;
			Send.ZC_NORMAL.SkillCancel(caster, skill.Id);
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, "BlitzHunter_VoltStrike_Pad", skill, effectDestination, direction, NormalizeEffectAngle(direction), 0f, effectHandle, EffectDistance, false, visualSkillLevel: 2);

		}

		private async System.Threading.Tasks.Task EmitRangeDebugBurst(Skill skill, ICombatEntity caster, Position fromPosition, Position destination, Direction fallbackDirection, TimeSpan duration, bool isBlitzkrieg)
		{
			var interval = isBlitzkrieg ? BlitzkriegRangeDebugInterval : RangeDebugInterval;
			var stepCount = Math.Max(1, (int)Math.Ceiling(duration.TotalMilliseconds / interval.TotalMilliseconds));
			if (isBlitzkrieg)
				stepCount = Math.Max(stepCount, BlitzkriegRangeDebugSteps);
			var waited = TimeSpan.Zero;

			for (var step = 0; step < stepCount; step++)
			{
				var t = (stepCount == 1 ? 1f : (step + 1f) / stepCount);
				var currentPosition = Position.Lerp(fromPosition, destination, t);
				var direction = fromPosition.GetDirection(currentPosition);
				if (direction == Direction.Zero)
					direction = fallbackDirection;

				Send.ZC_SKILL_RANGE_DBG(caster, currentPosition, direction, Position.Zero, RangeDebugF1, RangeDebugF2, RangeDebugF3, RangeDebugF4, RangeDebugF5, RangeDebugF6, includeCaster: true);

				var remaining = duration - waited;
				if (remaining <= TimeSpan.Zero)
					continue;

				var stepDelay = (step < stepCount - 1
					? TimeSpan.FromMilliseconds(Math.Min(interval.TotalMilliseconds, remaining.TotalMilliseconds))
					: remaining);

				if (stepDelay > TimeSpan.Zero)
				{
					await skill.Wait(stepDelay);
					waited += stepDelay;
				}
			}
		}

		private async System.Threading.Tasks.Task EmitRepeatedHitInfo(Skill skill, ICombatEntity caster, System.Collections.Generic.IReadOnlyList<HitInfo> hits)
		{
			if (hits.Count == 0)
				return;

			foreach (var hit in hits)
				Send.ZC_HIT_INFO(caster, hit.Target, hit);

			await System.Threading.Tasks.Task.CompletedTask;
		}

		private static float NormalizeEffectAngle(Direction direction)
		{
			var angle = -(float)Math.Atan2(direction.Sin, direction.Cos);
			while (angle > MathF.PI / 2f)
				angle -= MathF.PI;
			while (angle < -MathF.PI / 2f)
				angle += MathF.PI;
			return angle;
		}

		private static byte[] CreateRetailNoticePayload(int noticeIndex)
		{
			var source = RetailNoticePayloads[Math.Abs(noticeIndex) % RetailNoticePayloads.Length];
			return (byte[])source.Clone();
		}

		private System.Collections.Generic.IReadOnlyList<ICombatEntity> GetVoltStrikeTargets(ICombatEntity caster, Skill skill, Direction direction, ICombatEntity initialTarget)
		{
			var targets = new System.Collections.Generic.List<ICombatEntity>();

			if (initialTarget != null && !initialTarget.IsDead && caster.IsEnemy(initialTarget))
				targets.Add(initialTarget);

			var forwardTargets = BlitzHunterSkillHelper.GetForwardTargets(caster, skill, direction, skill.Properties.GetFloat(PropertyName.MaxR), 14f, MaxTargets);
			foreach (var target in forwardTargets)
			{
				if (target == null || target.IsDead || targets.Contains(target))
					continue;

				targets.Add(target);
				if (targets.Count >= MaxTargets)
					break;
			}

			return targets;
		}

		private System.Collections.Generic.List<HitInfo> PrepareDirectHits(ICombatEntity caster, Skill skill, System.Collections.Generic.IEnumerable<ICombatEntity> targets, SkillModifier modifier, TimeSpan aniTime, int hitCountOverride)
		{
			var hits = new System.Collections.Generic.List<HitInfo>();

			foreach (var target in targets)
			{
				if (target == null || target.IsDead)
					continue;

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				var hit = new HitInfo(caster, target, skill, skillHitResult, aniTime);
				hit.HitCount = hitCountOverride;
				hits.Add(hit);
			}

			return hits;
		}

		private void ApplyPreparedHits(ICombatEntity caster, System.Collections.Generic.IEnumerable<HitInfo> hits)
		{
			foreach (var hit in hits)
			{
				if (hit.Target == null || hit.Target.IsDead)
					continue;

				hit.Target.TakeDamage(hit.Damage, caster);
			}
		}

	}
}
