using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Network.Helpers;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.AetherBlader
{
	public static class AetherBladerSkillHelper
	{
		public const int MaxPuddles = 5;
		public const float PuddleRadius = 45f;
		public const float FrostPillarRadius = 45f;
		public const float ArcaneCollapseRadius = 95f;
		public const float ArcaneCollapseVisualBurstRadius = 30f;
		public const short AquareavePuddleGroundEffectKey = unchecked((short)0x8B38);
		public const short AquaRiftPuddleGroundEffectKey = unchecked((short)0xCD48);
		public const short AetherGroundImpactEffectKey = unchecked((short)0xAB70);
		public const string AetherBladeEffect = "aetherblader_blade";
		public const string AetherIceBladeEffect = "aetherblader_blade_blue";
		public const string AetherElectricBladeEffect = "aetherblader_blade_yellow";
		public const string SuppressDeathSkillCancelVar = "GuiltineSin.SuppressDeathSkillCancel";
		public static readonly TimeSpan PuddleDuration = TimeSpan.FromSeconds(15);
		public static readonly TimeSpan DrenchedDuration = TimeSpan.FromSeconds(10);
		public static readonly TimeSpan FrozenDuration = TimeSpan.FromSeconds(3);
		public static readonly TimeSpan TideCallDuration = TimeSpan.FromSeconds(30);
		private static readonly object ArcaneCollapseEffectLock = new();
		private static readonly Dictionary<int, DateTime> LastArcaneCollapseOpeningEffect = new();
		private static readonly Lazy<object> PuddleReservationLock = new(() => new object());
		private static readonly Lazy<Dictionary<int, int>> ReservedPuddleSlots = new(() => new Dictionary<int, int>());

		private static readonly HashSet<SkillId> AetherBladerDamageSkills =
		[
			SkillId.AetherBlader_Aquareave_Wizard,
			SkillId.AetherBlader_Aquareave_Cleric,
			SkillId.AetherBlader_Aquareave_Scout,
			SkillId.AetherBlader_AquaRift_Wizard,
			SkillId.AetherBlader_AquaRift_Cleric,
			SkillId.AetherBlader_AquaRift_Scout,
			SkillId.AetherBlader_FrostShatter_Wizard,
			SkillId.AetherBlader_FrostShatter_Cleric,
			SkillId.AetherBlader_FrostShatter_Scout,
			SkillId.AetherBlader_Frimveil_Wizard,
			SkillId.AetherBlader_Frimveil_Cleric,
			SkillId.AetherBlader_Frimveil_Scout,
			SkillId.AetherBlader_ArcaneCollapse_Wizard,
			SkillId.AetherBlader_ArcaneCollapse_Cleric,
			SkillId.AetherBlader_ArcaneCollapse_Scout,
		];

		public static bool TrySpendSkillSp(ICombatEntity caster, Skill skill, float multiplier = 1f)
		{
			if (caster.TrySpendSp(skill.SpendSp * multiplier))
				return true;

			caster.ServerMessage(Localization.Get("Not enough SP."));
			return false;
		}

		public static Direction GetDirection(ICombatEntity caster, Position originPos, Position farPos)
		{
			var direction = originPos.GetDirection(farPos);
			if (direction == Direction.Zero)
				direction = caster.Direction;
			return direction == Direction.Zero ? Direction.South : direction;
		}

		public static Direction GetCasterForwardDirection(ICombatEntity caster)
			=> caster.Direction == Direction.Zero ? Direction.South : caster.Direction;

		public static Position GetForwardPosition(ICombatEntity caster, float distance)
		{
			var direction = GetCasterForwardDirection(caster);
			var position = caster.Position.GetRelative(direction, Math.Max(0f, distance));
			return caster.Map.Ground.GetLastValidPosition(caster.Position, position);
		}

		public static Position ClampTargetPosition(ICombatEntity caster, Skill skill, Position farPos)
		{
			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR);
			if (maxRange <= 0)
				return farPos;

			if (caster.Position.Get2DDistance(farPos) <= maxRange)
				return farPos;

			var direction = GetDirection(caster, caster.Position, farPos);
			return caster.Position.GetRelative(direction, maxRange);
		}

		public static bool IsTargetInRange(ICombatEntity caster, Skill skill, ICombatEntity target)
		{
			if (target == null)
				return true;

			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR);
			return maxRange <= 0 || caster.Position.Get2DDistance(target.Position) <= maxRange + SizeTypeRadius.GetRadius(caster.EffectiveSize);
		}

		public static void PrepareGroundSkill(Skill skill, ICombatEntity caster, Position originPos, Position targetPos, Direction direction, ICombatEntity target = null)
		{
			var targetHandle = target?.Handle ?? 0;
			caster.SetAttackState(true);
			Send.ZC_SKILL_READY(caster, skill, caster.Position, targetPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, targetHandle, originPos, direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos, ForceId.GetNew(), null);
		}

		public static List<ICombatEntity> GetTargets(ICombatEntity caster, Skill skill, ISplashArea area)
			=> caster.Map.GetAttackableEnemiesIn(caster, area, hitType: skill.Data.HitType).LimitBySDR(caster, skill).ToList();

		public static List<SkillHitInfo> DealHits(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets, Func<ICombatEntity, SkillModifier> modifierFactory = null, TimeSpan? aniTime = null)
		{
			var hits = new List<SkillHitInfo>();
			var hitTargets = new HashSet<ICombatEntity>();
			var hitCount = Math.Max(1, skill.Properties.MultiHitCount);
			var hitDelay = skill.Properties.HitDelay;
			var displayDelay = aniTime ?? TimeSpan.FromMilliseconds(Math.Max(0, skill.Data.DefaultHitDelay.TotalMilliseconds));

			foreach (var target in targets)
			{
				var modifier = modifierFactory?.Invoke(target) ?? SkillModifier.MultiHit(hitCount);
				var result = SCR_SkillHit(caster, target, skill, modifier);
				target.SetTempVar(SuppressDeathSkillCancelVar, 1f);
				hitTargets.Add(target);
				target.TakeDamage(result.Damage, caster);
				hits.Add(new SkillHitInfo(caster, target, skill, result, displayDelay, hitDelay));
			}

			foreach (var target in hitTargets)
			{
				if (!target.IsDead)
					target.RemoveTempVar(SuppressDeathSkillCancelVar);
			}

			if (hits.Count > 0)
				Send.ZC_SKILL_HIT_INFO(caster, hits);

			return hits;
		}

		public static void ApplyDrenched(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets)
		{
			foreach (var target in targets)
				target.StartBuff(BuffId.Wet_Debuff, skill.Level, 0, DrenchedDuration, caster, skill.Id);
		}

		public static SkillModifier BuildModifier(Skill skill, ICombatEntity target, float extraFinalDamage = 0f)
		{
			var modifier = SkillModifier.MultiHit(Math.Max(1, skill.Properties.MultiHitCount));
			modifier.FinalDamageMultiplier += extraFinalDamage;
			return modifier;
		}

		public static float GetTideCallFinalDamageBonus(ICombatEntity caster)
		{
			if (!caster.TryGetBuff(BuffId.Tidecall_Buff, out var buff))
				return 0f;

			return Math.Max(0f, buff.NumArg2);
		}

		public static bool IsAetherBladerDamageSkill(SkillId skillId)
			=> AetherBladerDamageSkills.Contains(skillId);

		public static IReadOnlyList<Pad> GetPuddles(ICombatEntity caster)
			=> caster.Map.GetPads(pad => pad.Name == PadName.AetherBlader_Puddle_pad && pad.Creator?.Handle == caster.Handle && !pad.IsDead);

		public static int GetAvailablePuddleSlots(ICombatEntity caster)
			=> Math.Max(0, MaxPuddles - GetPuddles(caster).Count - GetReservedPuddleSlots(caster));

		public static int ReservePuddleSlots(ICombatEntity caster, int requestedSlots)
		{
			if (requestedSlots <= 0)
				return 0;

			lock (PuddleReservationLock.Value)
			{
				var availableSlots = Math.Max(0, MaxPuddles - GetPuddles(caster).Count - GetReservedPuddleSlotsUnsafe(caster));
				var reservedSlots = Math.Min(requestedSlots, availableSlots);
				if (reservedSlots <= 0)
					return 0;

				ReservedPuddleSlots.Value[caster.Handle] = GetReservedPuddleSlotsUnsafe(caster) + reservedSlots;
				return reservedSlots;
			}
		}

		public static void ReleasePuddleSlots(ICombatEntity caster, int releasedSlots)
		{
			if (releasedSlots <= 0)
				return;

			lock (PuddleReservationLock.Value)
			{
				var reservedSlots = GetReservedPuddleSlotsUnsafe(caster) - releasedSlots;
				if (reservedSlots > 0)
					ReservedPuddleSlots.Value[caster.Handle] = reservedSlots;
				else
					ReservedPuddleSlots.Value.Remove(caster.Handle);
			}
		}

		private static int GetReservedPuddleSlots(ICombatEntity caster)
		{
			lock (PuddleReservationLock.Value)
				return GetReservedPuddleSlotsUnsafe(caster);
		}

		private static int GetReservedPuddleSlotsUnsafe(ICombatEntity caster)
			=> ReservedPuddleSlots.Value.TryGetValue(caster.Handle, out var reservedSlots) ? reservedSlots : 0;

		private static bool TryConsumeReservedPuddleSlot(ICombatEntity caster)
		{
			lock (PuddleReservationLock.Value)
			{
				var reservedSlots = GetReservedPuddleSlotsUnsafe(caster);
				if (reservedSlots <= 0)
					return false;

				if (reservedSlots == 1)
					ReservedPuddleSlots.Value.Remove(caster.Handle);
				else
					ReservedPuddleSlots.Value[caster.Handle] = reservedSlots - 1;

				return true;
			}
		}

		public static bool TryGetPuddleAt(ICombatEntity caster, Position position, float range, out Pad puddle)
		{
			puddle = GetPuddles(caster)
				.Where(pad => pad.Position.Get2DDistance(position) <= range)
				.OrderBy(pad => pad.Position.Get2DDistance(position))
				.FirstOrDefault();

			return puddle != null;
		}

		public static IEnumerable<Position> GetPuddleAreaCheckPoints(Position position)
		{
			yield return position.GetRelative(Direction.North, PuddleRadius);
			yield return position.GetRelative(Direction.East, PuddleRadius);
			yield return position.GetRelative(Direction.South, PuddleRadius);
			yield return position.GetRelative(Direction.West, PuddleRadius);
		}

		public static bool IsPuddleReachedByArea(ISplashArea area, Pad puddle)
		{
			if (area.IsInside(puddle.Position))
				return true;

			foreach (var point in GetPuddleAreaCheckPoints(puddle.Position))
			{
				if (area.IsInside(point))
					return true;
			}

			return false;
		}

		public static void ExtendPuddles(ICombatEntity caster, TimeSpan extension)
		{
			foreach (var puddle in GetPuddles(caster))
				puddle.Trigger.LifeTime = puddle.Trigger.RemainingLifeTime + extension;
		}

		public static void SendAetherGroundImpactEffect(ICombatEntity caster, Position position)
		{
			Send.ZC_GROUND_EFFECT(caster, position, "F_rize024_violet", scale: 1f, duration: 1f, s2: AetherGroundImpactEffectKey);
		}

		public static void SendAetherBladeCastEffect(ICombatEntity caster)
		{
			caster.PlayEffectNode("eff_ark_dispersion_02", 0.8f, "Dummy_emitter");
		}

		public static void SendAetherBladeHandEffect(ICombatEntity caster, string effectName)
		{
			Send.ZC_NORMAL.Unknown_52_AttachedVisualEffect(caster, effectName, 1f, "Dummy_R_HAND");
		}

		public static void SendArcaneCollapseOpeningEffects(ICombatEntity caster, Position position)
		{
			Send.ZC_NORMAL.AttachedVisualEffect(caster, "Hit_HorizonFlare_Yellow_01", 3f, "Dummy_R_HAND");
			SendArcaneCollapseGroundAura(caster, position, unchecked((int)0xC2D80000));
		}

		public static void SendArcaneCollapseGroundEffects(ICombatEntity caster, Position position)
		{
			if (ShouldSendArcaneCollapseOpeningEffect(caster))
				SendArcaneCollapseOpeningEffects(caster, position);
			else
				SendArcaneCollapsePulseEffects(caster, position);
		}

		public static void SendArcaneCollapsePulseEffects(ICombatEntity caster, Position position)
			=> SendArcaneCollapsePulseEffects(caster, position, unchecked((int)0x3A78FFFF), 0x3A780000, false);

		public static void SendArcaneCollapsePulseEffects(ICombatEntity caster, Position position, int firstVariant, int secondVariant, bool includeAura)
		{
			SendArcaneCollapseLightningBursts(caster, position, firstVariant, secondVariant);
			if (includeAura)
				SendArcaneCollapseGroundAura(caster, position, secondVariant);
		}

		public static void SendArcaneCollapseLightningBursts(ICombatEntity caster, Position position, int firstVariant, int secondVariant)
		{
			SendArcaneCollapseLightningBurst(caster, position, firstVariant);
			SendArcaneCollapseLightningBurst(caster, position, secondVariant);
		}

		private static void SendArcaneCollapseLightningBurst(ICombatEntity caster, Position position, int variant)
		{
			var angle = Random.Shared.NextDouble() * 360;
			var distance = 8f + (float)Random.Shared.NextDouble() * ArcaneCollapseVisualBurstRadius;
			var burstPosition = position.GetRelative(new Direction(angle), distance);
			burstPosition = caster.Map.Ground.GetLastValidPosition(position, burstPosition);
			Send.ZC_UNITY_GROUND_EFFECT(caster, variant, 0x001F, 0.5f, burstPosition, 0f, 0f, 0f, new Direction(1f, 0f));
		}

		private static bool ShouldSendArcaneCollapseOpeningEffect(ICombatEntity caster)
		{
			lock (ArcaneCollapseEffectLock)
			{
				var now = DateTime.UtcNow;
				if (!LastArcaneCollapseOpeningEffect.TryGetValue(caster.Handle, out var last) || now - last > TimeSpan.FromSeconds(2))
				{
					LastArcaneCollapseOpeningEffect[caster.Handle] = now;
					return true;
				}

				return false;
			}
		}

		public static void SendArcaneCollapseGroundAura(ICombatEntity caster, Position position)
		{
			SendArcaneCollapseGroundAura(caster, position, 0);
		}

		public static void SendArcaneCollapseGroundAura(ICombatEntity caster, Position position, int variant)
		{
			Send.ZC_UNITY_GROUND_EFFECT(caster, variant, "GroundAura_ElectricAttackRange_Yellow_01".GetStringId(), 0.8f, position, 0f, 0f, 0f, new Direction(1f, 0f));
		}

		public static Pad CreatePuddle(ICombatEntity caster, Skill skill, Position position, short groundEffectKey = AquareavePuddleGroundEffectKey, bool sendPadVisual = true, bool consumeReservedSlot = false, Direction visualDirection = default, float visualArg1 = 0f, float visualArg2 = 0f, float visualArg3 = 40f, int? visualSkillLevel = null)
		{
			var hasReservedSlot = consumeReservedSlot && TryConsumeReservedPuddleSlot(caster);
			if (!hasReservedSlot && GetPuddles(caster).Count >= MaxPuddles)
				return null;

			var pad = Pad.Create(PadName.AetherBlader_Puddle_pad, caster, skill, position, new Circle(position, PuddleRadius), new PadOptions
			{
				LifeTime = PuddleDuration,
				UpdateInterval = TimeSpan.FromMilliseconds(500),
				MaxActorCount = 10,
			});
			pad.Direction = visualDirection == Direction.Zero ? GetCasterForwardDirection(caster) : visualDirection;
			pad.NumArg1 = visualArg1;
			pad.NumArg2 = visualArg2;
			pad.NumArg3 = visualArg3;

			caster.Map.AddPad(pad);
			Send.ZC_GROUND_EFFECT(caster, position, "", scale: 1.2f, s2: groundEffectKey);
			Send.ZC_GROUND_EFFECT(caster, position, "", scale: 1.2f, s2: groundEffectKey);
			if (sendPadVisual)
				Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.AetherBlader_Puddle_pad, skill, position, pad.Direction, pad.NumArg1, pad.NumArg2, pad.Handle, pad.NumArg3, true, visualSkillLevel: visualSkillLevel);
			return pad;
		}

		public static void DestroyPuddle(Pad puddle)
		{
			if (puddle == null || puddle.IsDead)
				return;

			puddle.Destroy();
		}

		public static Pad CreateFrostPillar(ICombatEntity caster, Skill skill, Position position)
		{
			var pad = Pad.Create(PadName.AetherBlader_FrostShatter, caster, skill, position, new Circle(position, FrostPillarRadius), new PadOptions
			{
				LifeTime = TimeSpan.FromSeconds(1.5),
				UpdateInterval = TimeSpan.FromMilliseconds(250),
				MaxActorCount = 10,
				NumArg1 = GetFrostPillarAngleArg(caster, position),
				NumArg2 = (float)caster.Position.Get2DDistance(position),
				NumArg3 = 40f,
			});
			pad.Direction = Direction.East;

			caster.Map.AddPad(pad);
			return pad;
		}

		private static float GetFrostPillarAngleArg(ICombatEntity caster, Position position)
		{
			var direction = caster.Position.GetDirection(position);
			if (direction == Direction.Zero)
				direction = GetCasterForwardDirection(caster);

			return (float)(Math.PI / 2 - Math.Atan2(direction.Sin, direction.Cos));
		}
	}
}
