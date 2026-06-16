using System;
using System.Collections.Generic;
using System.Linq;
using Melia.Shared.Data.Database;
using Melia.Shared.Game.Const;
using Melia.Shared.L10N;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.Characters;
using Melia.Zone.World.Actors.Components;
using static Melia.Zone.Skills.SkillUseFunctions;
using System.Threading.Tasks;

namespace Melia.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	public static class BlitzHunterSkillHelper
	{
		private static readonly Dictionary<SkillId, float> BlitzkriegShootTimeOverrides =
			new()
			{
					{ SkillId.BlitzHunter_VoltStrike_Archer, 500f },
					{ SkillId.BlitzHunter_VoltStrike_Scout, 500f },
					{ SkillId.BlitzHunter_VoltChain_Archer, 500f },
					{ SkillId.BlitzHunter_VoltChain_Scout, 500f },
					{ SkillId.BlitzHunter_TempestShot_Archer, 1350f },
					{ SkillId.BlitzHunter_TempestShot_Scout, 1350f },
					{ SkillId.BlitzHunter_ElectricSurge_Archer, 300f },
					{ SkillId.BlitzHunter_ElectricSurge_Scout, 300f },
			};

		private static readonly Dictionary<SkillId, TimeSpan> BlitzkriegReducedCooldowns =
			new()
			{
					{ SkillId.BlitzHunter_VoltStrike_Archer, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_VoltStrike_Scout, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_VoltChain_Archer, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_VoltChain_Scout, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_TempestShot_Archer, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_TempestShot_Scout, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_ElectricSurge_Archer, TimeSpan.FromSeconds(3) },
					{ SkillId.BlitzHunter_ElectricSurge_Scout, TimeSpan.FromSeconds(3) },
			};

		private static readonly HashSet<SkillId> BlitzHunterDamageSkills =
		[
		SkillId.BlitzHunter_VoltStrike_Archer,
			SkillId.BlitzHunter_VoltStrike_Scout,
			SkillId.BlitzHunter_VoltChain_Archer,
			SkillId.BlitzHunter_VoltChain_Scout,
			SkillId.BlitzHunter_TempestShot_Archer,
			SkillId.BlitzHunter_TempestShot_Scout,
			SkillId.BlitzHunter_Lightningrod_Archer,
			SkillId.BlitzHunter_Lightningrod_Scout,
			SkillId.BlitzHunter_ElectricSurge_Archer,
			SkillId.BlitzHunter_ElectricSurge_Scout,
		];

		private static readonly SkillId[][] BlitzkriegAffectedSkillGroups =
		[
			[
				SkillId.BlitzHunter_VoltStrike_Scout,
				SkillId.BlitzHunter_VoltStrike_Archer,
			],
			[
				SkillId.BlitzHunter_VoltChain_Scout,
				SkillId.BlitzHunter_VoltChain_Archer,
			],
			[
				SkillId.BlitzHunter_TempestShot_Scout,
				SkillId.BlitzHunter_TempestShot_Archer,
			],
			[
				SkillId.BlitzHunter_ElectricSurge_Scout,
				SkillId.BlitzHunter_ElectricSurge_Archer,
			],
		];

		private static readonly TimeSpan VolticCatharsisDuration = TimeSpan.FromSeconds(60);

		public static bool IsBlitzHunterDamageSkill(SkillId skillId)
			=> BlitzHunterDamageSkills.Contains(skillId);

		public static IReadOnlyList<SkillId> GetBlitzkriegAffectedSkills(ICombatEntity caster)
		{
			if (caster is not Character character)
				return Array.Empty<SkillId>();

			var result = new List<SkillId>(BlitzkriegAffectedSkillGroups.Length);

			foreach (var skillGroup in BlitzkriegAffectedSkillGroups)
			{
				var ownedSkillId = skillGroup.FirstOrDefault(skillId => character.HasSkill(skillId));
				if (ownedSkillId != 0)
					result.Add(ownedSkillId);
			}

			return result;
		}

		public static bool TryGetBlitzkriegReducedCooldown(SkillId skillId, out TimeSpan cooldown)
			=> BlitzkriegReducedCooldowns.TryGetValue(skillId, out cooldown);

		public static void InvalidateBlitzkriegSkillProperties(ICombatEntity caster)
		{
			if (caster is not Character character)
				return;

			foreach (var skillId in GetBlitzkriegAffectedSkills(caster))
			{
				if (!character.TryGetSkill(skillId, out var skill))
					continue;

				skill.Properties.Invalidate(PropertyName.ShootTime);
			}
		}

		public static float GetEffectiveShootTimeMs(Skill skill)
		{
			var baseShootTime = (float)skill.Data.ShootTime.TotalMilliseconds;

			if (!skill.Owner.IsBuffActive(BuffId.Blitzkrieg_Buff))
				return baseShootTime;

			if (BlitzkriegShootTimeOverrides.TryGetValue(skill.Id, out var overrideShootTime))
				return overrideShootTime;

			return baseShootTime;
		}

		public static float GetAdditionalTimingRate(Skill skill)
		{
			var baseShootTime = (float)skill.Data.ShootTime.TotalMilliseconds;
			if (baseShootTime <= 0f)
				return 1f;

			var effectiveShootTime = GetEffectiveShootTimeMs(skill);
			if (effectiveShootTime <= 0f)
				return 1f;

			return baseShootTime / effectiveShootTime;
		}

		public static TimeSpan GetEffectiveShootTime(Skill skill)
		{
			var effectiveShootTime = GetEffectiveShootTimeMs(skill);
			if (effectiveShootTime <= 0f)
				return TimeSpan.Zero;

			return TimeSpan.FromMilliseconds(effectiveShootTime);
		}

		public static void ApplySkillAnimationMovementLock(ICombatEntity caster, Skill skill, int extraMs = 0)
		{
			if (caster == null || skill == null)
				return;

			var duration = GetEffectiveShootTime(skill);
			if (extraMs > 0)
				duration += TimeSpan.FromMilliseconds(extraMs);

			if (duration > TimeSpan.Zero)
				caster.Lock(LockType.Movement, duration);
		}

		public static void SendGroundSkillReady(ICombatEntity caster, Skill skill)
		{
			Send.ZC_SKILL_READY(caster, skill, caster.Position, caster.Position);
		}

		public static void SendForceTargetSkillReady(ICombatEntity caster, Skill skill, ICombatEntity target)
		{
			var targetPosition = target?.Position ?? caster.Position;
			Send.ZC_SKILL_READY(caster, skill, caster.Position, targetPosition);
		}

		public static void SendBlitzkriegSkillState(ICombatEntity caster)
		{
			if (caster is not Character character)
				return;

			var skills = GetBlitzkriegAffectedSkills(caster)
				.Select(skillId => character.TryGetSkill(skillId, out var skill) ? skill : null)
				.Where(skill => skill != null)
				.ToArray();

			if (skills.Length == 0)
				return;

			Send.ZC_UPDATE_SKL_SPDRATE_LIST(character, skills);
		}

		public static bool IsBlitzkriegActive(Skill skill)
			=> skill.Owner.IsBuffActive(BuffId.Blitzkrieg_Buff);

		public static bool TrySpendSkillSp(ICombatEntity caster, Skill skill, float multiplier = 1f)
		{
			var spendSp = skill.SpendSp * multiplier;
			if (caster.TrySpendSp(spendSp))
				return true;

			caster.ServerMessage(Localization.Get("Not enough SP."));
			return false;
		}

		public static bool HasPacketString(string packetString)
		{
			if (string.IsNullOrWhiteSpace(packetString) || packetString == "None")
				return true;

			var normalized = NormalizePacketString(packetString);
			return ZoneServer.Instance.Data.PacketStringDb.TryFind(normalized, out _);
		}

		public static string NormalizePacketString(string packetString)
		{
			if (string.IsNullOrWhiteSpace(packetString) || packetString == "None")
				return "None";

			return packetString.Split('#')[0];
		}

		public static void TryPlaySound(IActor actor, string packetString, bool loop = false, float volumeFix = -1f, bool useSetBalanceVolume = false)
		{
			if (actor == null || string.IsNullOrWhiteSpace(packetString) || packetString == "None")
				return;

			if (!HasPacketString(packetString))
				return;

			Send.ZC_PLAY_SOUND(actor, packetString, loop, volumeFix, useSetBalanceVolume);
		}

		public static void TryPlayAnimation(IActor actor, string animationName, bool stopOnLastFrame = false, float readyTime = 0f, float animationSpeed = 1f, byte b1 = 0)
		{
			if (actor == null || !HasPacketString(animationName))
				return;

			Send.ZC_PLAY_ANI(actor, animationName, stopOnLastFrame, readyTime, animationSpeed, b1);
		}

		public static void TryPlayEffectNode(IActor actor, string effectName, float duration, string str1 = "None", string str2 = "None")
		{
			if (actor == null || !HasPacketString(effectName))
				return;

			actor.PlayEffectNode(effectName, duration, str1, str2);
		}

		public static void TryPlayEffect(IActor actor, string effectName, float scale = 1f, EffectLocation heightOffset = EffectLocation.Bottom, byte b1 = 1, byte b2 = 0, int associatedHandle = 0)
		{
			if (actor == null || !HasPacketString(effectName))
				return;

			actor.PlayEffect(effectName, scale, b1, heightOffset, b2, associatedHandle);
		}

		public static void PlayVoltChainTargetEffect(IActor target)
		{
			if (target == null)
				return;

			TryPlayEffect(target, "Shoot_ElectricShock_Blue_01", 0.7f, EffectLocation.Top);
		}

		public static void TryPlayForceEffect(int forceId, IActor caster, IActor source, IActor target,
			string effect, float scale, string soundEffect, string endEffect, float endEffectScale,
			string endSoundEffect, string effectSpeed, float speed)
		{
			if (caster == null || source == null || target == null)
				return;

			if (!HasPacketString(effect) || !HasPacketString(soundEffect) || !HasPacketString(endEffect) ||
				!HasPacketString(endSoundEffect) || !HasPacketString(effectSpeed))
				return;

			Send.ZC_NORMAL.PlayForceEffect(
				forceId,
				caster,
				source,
				target,
				NormalizePacketString(effect),
				scale,
				NormalizePacketString(soundEffect),
				NormalizePacketString(endEffect),
				endEffectScale,
				NormalizePacketString(endSoundEffect),
				NormalizePacketString(effectSpeed),
				speed
			);
		}

		public static async Task FinishCastAsync(Skill skill, ICombatEntity caster, TimeSpan elapsed, bool stopAnimation = false)
		{
			var remainingAnimation = GetEffectiveShootTime(skill) - elapsed;
			if (remainingAnimation > TimeSpan.Zero)
				await skill.Wait(remainingAnimation);

			if (stopAnimation)
				Send.ZC_NORMAL.StopAnimation(caster);

			Send.ZC_NORMAL.ResetStdAnim(caster);
		}

		public static TimeSpan GetVariantDelay(Skill skill, int normalMs, int blitzkriegMs)
			=> TimeSpan.FromMilliseconds(IsBlitzkriegActive(skill) ? blitzkriegMs : normalMs);

		public static TimeSpan GetScaledHitDelay(Skill skill, int fallbackMs = 0)
		{
			var hitDelay = skill.Properties.HitDelay;
			if (hitDelay <= TimeSpan.Zero && fallbackMs > 0)
				hitDelay = TimeSpan.FromMilliseconds(fallbackMs);

			var timingRate = skill.Properties.GetFloatSafe(PropertyName.SklSpdRate) * GetAdditionalTimingRate(skill);
			if (timingRate > 0f)
				hitDelay = TimeSpan.FromMilliseconds(hitDelay.TotalMilliseconds / timingRate);

			return hitDelay;
		}

		public static TimeSpan GetScaledDisplayDelay(Skill skill, int baseMs)
		{
			if (baseMs <= 0)
				return TimeSpan.Zero;

			var timingRate = skill.Properties.GetFloatSafe(PropertyName.SklSpdRate) * GetAdditionalTimingRate(skill);
			if (timingRate <= 0f)
				return TimeSpan.FromMilliseconds(baseMs);

			return TimeSpan.FromMilliseconds(baseMs / timingRate);
		}

		public static int GetConfiguredMultiHitCount(Skill skill, int fallback = 1)
		{
			if (skill == null)
				return fallback;

			var hitCount = skill.Properties.MultiHitCount;
			return hitCount > 0 ? hitCount : fallback;
		}

		public static int GetDistributedMultiHitCount(Skill skill, int stages, int fallbackPerStage = 1)
		{
			if (skill == null || stages <= 0)
				return fallbackPerStage;

			var totalHitCount = GetConfiguredMultiHitCount(skill, fallbackPerStage);
			return Math.Max(fallbackPerStage, totalHitCount / stages);
		}

		public static void ConsumeBlitzkriegStacks(ICombatEntity caster, Skill skill)
		{
			caster.StopBuff(BuffId.VolticCatharsis_Buff);
			caster.StartBuff(BuffId.VolticCatharsis_Debuff, skill.Level, 0, TimeSpan.FromMilliseconds(1200), caster, skill.Id);
		}

		public static bool ValidateForceTarget(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			if (target == null || target.IsDead)
			{
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill);
				return false;
			}

			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR);
			if (!caster.Position.InRange2D(target.Position, maxRange))
			{
				caster.ServerMessage(Localization.Get("Too far away."));
				Send.ZC_SKILL_FORCE_TARGET(caster, null, skill);
				return false;
			}

			return true;
		}

		public static Direction GetDirection(ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target = null)
		{
			if (target != null)
			{
				var targetDirection = caster.Position.GetDirection(target.Position);
				if (targetDirection != Direction.Zero)
					return targetDirection;
			}

			if (originPos != farPos)
			{
				var castDirection = originPos.GetDirection(farPos);
				if (castDirection != Direction.Zero)
					return castDirection;
			}

			return caster.Direction;
		}

		public static IReadOnlyList<ICombatEntity> GetForwardTargets(ICombatEntity caster, Skill skill, Direction direction, float length, float width, int maxTargets)
		{
			var splashParam = skill.GetSplashParameters(caster, caster.Position, caster.Position.GetRelative(direction, length), length, width, 0f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			return caster.Map.GetAttackableEnemiesIn(caster, splashArea, hitType: skill.Data.HitType)
				.LimitBySDR(caster, skill)
				.Limit(maxTargets)
				.ToList();
		}

		public static IReadOnlyList<ICombatEntity> GetCircleTargets(ICombatEntity caster, Skill skill, Position position, float radius, int maxTargets)
		{
			var circle = new Circle(position, radius);

			return caster.Map.GetAttackableEnemiesIn(caster, circle, hitType: skill.Data.HitType)
				.LimitBySDR(caster, skill)
				.Limit(maxTargets)
				.ToList();
		}

		public static IReadOnlyList<ICombatEntity> GetLineTargetsToTarget(ICombatEntity caster, Skill skill, ICombatEntity mainTarget, float length, float width, int maxTargets)
		{
			var splashParam = skill.GetSplashParameters(caster, caster.Position, mainTarget.Position, length, width, 0f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea, hitType: skill.Data.HitType)
				.Where(target => target != null && !target.IsDead)
				.ToList();

			var orderedTargets = new List<ICombatEntity>();
			if (targets.Remove(mainTarget))
				orderedTargets.Add(mainTarget);

			orderedTargets.AddRange(targets
				.OrderBy(target => target.Position.Get2DDistance(mainTarget.Position))
				.LimitBySDR(caster, skill)
				.Limit(maxTargets));

			return orderedTargets
				.Distinct()
				.Take(maxTargets)
				.ToList();
		}

		public static IReadOnlyList<ICombatEntity> GetBounceChainTargets(ICombatEntity caster, Skill skill, ICombatEntity mainTarget, float bounceRange, int maxTargets)
		{
			if (caster?.Map == null)
				return Array.Empty<ICombatEntity>();

			var chainTargets = new List<ICombatEntity>();
			var alreadyHit = new HashSet<ICombatEntity>();
			var currentTarget = mainTarget;
			ICombatEntity previousTarget = null;

			if (currentTarget == null || currentTarget.IsDead)
			{
				currentTarget = caster.Map.GetAttackableEnemiesInPosition(caster, caster.Position, skill.Properties.GetFloat(PropertyName.MaxR))
					.Where(t => t != null && !t.IsDead)
					.OrderBy(t => t.Position.Get2DDistance(caster.Position))
					.FirstOrDefault();
			}

			for (var bounce = 0; bounce < maxTargets; bounce++)
			{
				if (currentTarget == null || currentTarget.IsDead || !caster.IsEnemy(currentTarget))
					break;

				chainTargets.Add(currentTarget);
				alreadyHit.Add(currentTarget);

				var nearbyTargets = currentTarget.Map.GetAttackableEnemiesInPosition(caster, currentTarget.Position, bounceRange)
					.Where(t => t != null && t != currentTarget && !t.IsDead);

				var nextTarget = nearbyTargets
					.Where(t => !alreadyHit.Contains(t))
					.OrderBy(t => t.Position.Get2DDistance(currentTarget.Position))
					.FirstOrDefault();

				if (nextTarget == null)
				{
					nextTarget = nearbyTargets
						.Where(t => t != previousTarget)
						.OrderBy(t => t.Position.Get2DDistance(currentTarget.Position))
						.FirstOrDefault();
				}

				previousTarget = currentTarget;
				currentTarget = nextTarget;
			}

			return chainTargets;
		}

		public static List<SkillHitInfo> DealHits(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets, SkillModifier modifier, TimeSpan aniTime, TimeSpan hitDelay, int sharedForceId = 0)
		{
			var hits = new List<SkillHitInfo>();

			foreach (var target in targets)
			{
				if (target == null || target.IsDead)
					continue;

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				target.TakeDamage(skillHitResult.Damage, caster);

				var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, hitDelay);
				if (sharedForceId != 0)
					skillHit.ForceId = sharedForceId;

				hits.Add(skillHit);
			}

			return hits;
		}

		public static List<HitInfo> DealDirectHits(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets, SkillModifier modifier, TimeSpan aniTime, int sharedForceId = 0, int? hitCountOverride = null, float unkFloat1 = 0f, float unkFloat2 = 0f)
		{
			var hits = new List<HitInfo>();

			foreach (var target in targets)
			{
				if (target == null || target.IsDead)
					continue;

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				target.TakeDamage(skillHitResult.Damage, caster);

				var hit = new HitInfo(caster, target, skill, skillHitResult, aniTime)
				{
					ForceId = sharedForceId,
					UnkFloat1 = unkFloat1,
					UnkFloat2 = unkFloat2,
				};

				if (hitCountOverride.HasValue)
					hit.HitCount = hitCountOverride.Value;

				hits.Add(hit);
			}

			return hits;
		}

		public static bool HasAccurateHit(IEnumerable<SkillHitInfo> hits)
			=> hits.Any(hit => hit.HitInfo.Damage > 0 && hit.HitInfo.ResultType != HitResultType.Dodge);

		public static bool HasAccurateHit(IEnumerable<HitInfo> hits)
			=> hits.Any(hit => hit.Damage > 0 && hit.ResultType != HitResultType.Dodge);

		public static void ApplyVolticCatharsis(ICombatEntity caster, Skill skill, int amount = 10)
		{
			if (caster.IsBuffActive(BuffId.Blitzkrieg_Buff) || caster.IsBuffActive(BuffId.VolticCatharsis_Debuff))
				return;

			var currentStacks = 0;
			if (caster.TryGetBuff(BuffId.VolticCatharsis_Buff, out var buff))
				currentStacks = buff.OverbuffCounter;

			caster.StartBuff(BuffId.VolticCatharsis_Buff, skill.Level, 0, VolticCatharsisDuration, caster, skill.Id, createdBuff =>
			{
				createdBuff.OverbuffCounter = currentStacks + amount;
			});
		}

		public static void ApplyVolticCatharsisImpact(ICombatEntity caster, Skill skill, int amount = 10)
		{
			if (caster.IsBuffActive(BuffId.Blitzkrieg_Buff))
				return;

			var currentStacks = 0;
			if (caster.TryGetBuff(BuffId.VolticCatharsis_Buff, out var existingBuff))
				currentStacks = existingBuff.OverbuffCounter;

			// Recreate both buffs on impact so the client receives BUFF_ADD for the
			// retail-like post-hit pair instead of updating a pre-existing state.
			caster.StopBuff(BuffId.VolticCatharsis_Buff, BuffId.VolticCatharsis_Debuff);

			caster.StartBuff(BuffId.VolticCatharsis_Debuff, skill.Level, 0, TimeSpan.FromMilliseconds(1200), caster, skill.Id);
			caster.StartBuff(BuffId.VolticCatharsis_Buff, skill.Level, 0, VolticCatharsisDuration, caster, skill.Id, createdBuff =>
			{
				createdBuff.OverbuffCounter = currentStacks + amount;
			});
		}

		public static int CreateRetailVisualPadHandle()
			=> CreateRetailVisualPadHandle(18000);

		public static int CreateRetailVisualPadHandle(int retailLikeFloor)
			=> retailLikeFloor + ZoneServer.Instance.World.CreatePadHandle();
	}
}
