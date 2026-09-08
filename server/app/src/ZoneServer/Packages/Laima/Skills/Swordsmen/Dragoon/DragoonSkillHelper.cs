using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	internal static class DragoonSkillHelper
	{
		public static bool StartGroundSkill(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			if (!HasSpear(caster))
			{
				caster.ServerMessage(Localization.Get("Skill requires a spear or two-handed spear."));
				Send.ZC_SKILL_DISABLE(caster);
				return false;
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return false;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();
			Send.ZC_SKILL_READY(caster, skill, skillHandle, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			return true;
		}

		public static bool StartSelfSkill(Skill skill, ICombatEntity caster, Position originPos)
		{
			if (!HasSpear(caster))
			{
				caster.ServerMessage(Localization.Get("Skill requires a spear or two-handed spear."));
				Send.ZC_SKILL_DISABLE(caster);
				return false;
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return false;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();
			Send.ZC_SKILL_READY(caster, skill, skillHandle, originPos, Position.Zero);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, originPos, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);

			return true;
		}

		public static async Task AttackArea(ICombatEntity caster, Skill skill, Position originPos, Position farPos, int length, int width, float delay, int hitCount = 1, Action<ICombatEntity, Skill, SkillHitInfo> afterHit = null, Func<Skill, ICombatEntity, ICombatEntity, SkillHitResult, SkillHitResult> modify = null, int lastHitExtraDelay = 0)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: length, width: width, angle: 10f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			await AttackSequential(caster, skill, splashArea, delay, hitCount, afterHit, modify, lastHitExtraDelay);
		}

		public static Task AttackForward(ICombatEntity caster, Skill skill, Position originPos, int length, int width, float delay, int hitCount = 1, Action<ICombatEntity, Skill, SkillHitInfo> afterHit = null, Func<Skill, ICombatEntity, ICombatEntity, SkillHitResult, SkillHitResult> modify = null, int lastHitExtraDelay = 0)
		{
			var forwardPos = originPos.GetRelative2D(caster.Direction, length);
			return AttackArea(caster, skill, originPos, forwardPos, length, width, delay, hitCount, afterHit, modify, lastHitExtraDelay);
		}

		public static Position GetForwardTargetPos(ICombatEntity caster, Position originPos, int length)
			=> originPos.GetRelative2D(caster.Direction, length);

		public static Position ClampTargetPosition(ICombatEntity caster, Position originPos, Position farPos, float maxDistance)
		{
			if (originPos.InRange2D(farPos, maxDistance))
				return farPos;

			return originPos.GetRelative2D(originPos.GetDirection(farPos), maxDistance);
		}

		public static bool HasCompletedFixedCast(Skill skill, string castStartedAtKey, TimeSpan castTime, int graceMs = 350)
		{
			if (!long.TryParse(skill.Vars.GetString(castStartedAtKey), out var startedAt))
				return false;

			var elapsedMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startedAt;
			return elapsedMs >= castTime.TotalMilliseconds - graceMs;
		}

		public static async Task AttackCircle(ICombatEntity caster, Skill skill, Position position, int radius, float delay, int hitCount = 1, Action<ICombatEntity, Skill, SkillHitInfo> afterHit = null, Func<Skill, ICombatEntity, ICombatEntity, SkillHitResult, SkillHitResult> modify = null)
		{
			await AttackSequential(caster, skill, new Circle(position, radius), delay, hitCount, afterHit, modify);
		}

		public static async Task ApplyDebuffCircle(ICombatEntity caster, Skill skill, Position position, int radius, float delay, Action<ICombatEntity, Skill, ICombatEntity> apply)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(delay));

			var targets = caster.Map.GetAttackableEnemiesIn(caster, new Circle(position, radius), hitType: skill.Data.HitType);
			foreach (var target in targets)
				apply(caster, skill, target);
		}

		public static bool IsHeldOrSlowed(ICombatEntity target)
		{
			return target.IsBuffActive(BuffId.Common_Slow)
				|| target.IsBuffActive(BuffId.DragonFear_Slow_Debuff)
				|| target.IsBuffActive(BuffId.Dethrone_Debuff)
				|| target.IsBuffActive(BuffId.DethroneBoss_Debuff)
				|| target.IsBuffActive(BuffId.Stun);
		}

		public static SkillHitResult ApplyDragontoothLongCanine(Skill skill, ICombatEntity attacker, ICombatEntity target, SkillHitResult result)
		{
			if (attacker.IsAbilityActive(AbilityId.Dragoon28))
				result.Damage *= 3f;

			return result;
		}

		public static SkillHitResult ApplySlowOrHoldBonus(Skill skill, ICombatEntity attacker, ICombatEntity target, SkillHitResult result)
		{
			if (IsHeldOrSlowed(target))
				result.Damage *= 1.15f;

			return result;
		}

		private static async Task AttackSequential(ICombatEntity caster, Skill skill, ISplashArea splashArea, float delay, int hitCount, Action<ICombatEntity, Skill, SkillHitInfo> afterHit, Func<Skill, ICombatEntity, ICombatEntity, SkillHitResult, SkillHitResult> modify, int lastHitExtraDelay = 0)
		{
			var firstDelay = TimeSpan.FromMilliseconds(delay);
			var hitInterval = TimeSpan.FromMilliseconds(90);

			for (var i = 0; i < Math.Max(1, hitCount); ++i)
			{
				var wait = i == 0 ? firstDelay : hitInterval;
				if (lastHitExtraDelay > 0 && i == hitCount - 1)
					wait += TimeSpan.FromMilliseconds(lastHitExtraDelay);

				await skill.Wait(wait);

				var hits = new List<SkillHitInfo>();
				await SkillAttack(caster, skill, splashArea, hitDelay: 0, aniTime: 0, hits, modifySkillHitResult: modify);

				if (afterHit == null)
					continue;

				foreach (var hit in hits)
					afterHit(caster, skill, hit);
			}
		}

		public static bool IsDragoonSkill(Skill skill)
		{
			return skill.Id == SkillId.Dragoon_Dragontooth
				|| skill.Id == SkillId.Dragoon_Serpentine
				|| skill.Id == SkillId.Dragoon_Gae_Bulg
				|| skill.Id == SkillId.Dragoon_Dragon_Soar
				|| skill.Id == SkillId.Dragoon_Dethrone
				|| skill.Id == SkillId.Dragoon_DragonFear
				|| skill.Id == SkillId.Dragoon_DragonFall
				|| skill.Id == SkillId.Dragoon_DragoonHelmet;
		}

		private static bool HasSpear(ICombatEntity caster)
		{
			if (caster is not Character character)
				return true;

			var weapon = character.Inventory.GetItem(EquipSlot.RightHand);
			if (weapon == null)
				return false;

			return weapon.Data.EquipType1 == EquipType.Spear || weapon.Data.EquipType1 == EquipType.THSpear;
		}
	}
}
