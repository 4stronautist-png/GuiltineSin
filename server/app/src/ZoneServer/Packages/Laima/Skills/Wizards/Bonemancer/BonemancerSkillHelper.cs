using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melia.Shared.Data.Database;
using Melia.Shared.Game.Const;
using Melia.Shared.L10N;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;
using static Melia.Zone.Skills.SkillUseFunctions;

namespace Melia.Zone.Skills.Handlers.Wizards.Bonemancer
{
	public static class BonemancerSkillHelper
	{
		public static readonly TimeSpan BasicDebuffDuration = TimeSpan.FromSeconds(20);
		public static readonly TimeSpan ReinforcedDebuffDuration = TimeSpan.FromSeconds(10);
		private static readonly TimeSpan DebuffInternalCooldown = TimeSpan.FromSeconds(1);

		private static readonly HashSet<SkillId> BonemancerDamageSkills =
		[
			SkillId.Bonemancer_BoneFist_Swordman, SkillId.Bonemancer_BoneFist_Wizard, SkillId.Bonemancer_BoneFist_Archer, SkillId.Bonemancer_BoneFist_Cleric,
			SkillId.Bonemancer_BoneReaper_Swordman, SkillId.Bonemancer_BoneReaper_Wizard, SkillId.Bonemancer_BoneReaper_Archer, SkillId.Bonemancer_BoneReaper_Cleric,
			SkillId.Bonemancer_BoneWhip_Swordman, SkillId.Bonemancer_BoneWhip_Wizard, SkillId.Bonemancer_BoneWhip_Archer, SkillId.Bonemancer_BoneWhip_Cleric,
			SkillId.Bonemancer_BoneStorm_Swordman, SkillId.Bonemancer_BoneStorm_Wizard, SkillId.Bonemancer_BoneStorm_Archer, SkillId.Bonemancer_BoneStorm_Cleric,
			SkillId.Bonemancer_BoneReinforcement_Swordman, SkillId.Bonemancer_BoneReinforcement_Wizard, SkillId.Bonemancer_BoneReinforcement_Archer, SkillId.Bonemancer_BoneReinforcement_Cleric,
		];

		public static bool TrySpendSp(ICombatEntity caster, Skill skill, float multiplier = 1f)
		{
			if (caster.TrySpendSp(skill.SpendSp * multiplier))
				return true;

			caster.ServerMessage(Localization.Get("Not enough SP."));
			return false;
		}

		public static bool IsBonemancerDamageSkill(SkillId skillId)
			=> BonemancerDamageSkills.Contains(skillId);

		public static Direction GetDirection(ICombatEntity caster, Position originPos, Position farPos)
		{
			var direction = originPos.GetDirection(farPos);
			if (direction == Direction.Zero)
				direction = caster.Direction;
			return direction == Direction.Zero ? Direction.South : direction;
		}

		public static Position ClampTargetPosition(ICombatEntity caster, Skill skill, Position farPos)
		{
			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR);
			if (maxRange <= 0 || caster.Position.Get2DDistance(farPos) <= maxRange)
				return farPos;

			return caster.Map.Ground.GetLastValidPosition(caster.Position, caster.Position.GetRelative(GetDirection(caster, caster.Position, farPos), maxRange));
		}

		public static void PrepareGroundSkill(Skill skill, ICombatEntity caster, Position originPos, Position targetPos, Direction direction)
		{
			caster.SetAttackState(true);
			Send.ZC_SKILL_READY(caster, skill, caster.Position, targetPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, originPos, direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos, ForceId.GetNew(), null);
		}

		public static void PlayRetailBonePulse(IActor caster)
			=> Send.ZC_NORMAL.PlayEffectNode(caster, "Death_LightDisintegration_Blue_01", 1f, "Dummy_emitter", "None", 600);

		public static async Task FinishRetailCast(Skill skill, ICombatEntity caster, TimeSpan delay, bool cancelSkill = false)
		{
			await skill.Wait(delay);
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
			if (cancelSkill)
				Send.ZC_NORMAL.SkillCancel(caster, skill.Id);
		}

		public static List<ICombatEntity> GetTargets(ICombatEntity caster, Skill skill, ISplashArea area, int maxTargets)
			=> caster.Map.GetAttackableEnemiesIn(caster, area, hitType: skill.Data.HitType)
				.LimitBySDR(caster, skill)
				.Take(maxTargets)
				.ToList();

		public static List<ICombatEntity> GetFixedCountTargets(ICombatEntity caster, Skill skill, ISplashArea area, int maxTargets)
			=> caster.Map.GetAttackableEnemiesIn(caster, area, hitType: skill.Data.HitType)
				.Take(maxTargets)
				.ToList();

		public static List<SkillHitInfo> DealHits(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets, SkillModifier modifier = null)
		{
			var hits = new List<SkillHitInfo>();
			foreach (var target in targets)
			{
				var result = SCR_SkillHit(caster, target, skill, modifier ?? SkillModifier.MultiHit(Math.Max(1, skill.Properties.MultiHitCount)));
				target.TakeDamage(result.Damage, caster);
				hits.Add(new SkillHitInfo(caster, target, skill, result, TimeSpan.Zero, skill.Properties.HitDelay));
			}

			if (hits.Count > 0)
				Send.ZC_SKILL_HIT_INFO(caster, hits);

			return hits;
		}

		public static void ApplyBasicDebuffs(ICombatEntity caster, Skill skill, IEnumerable<ICombatEntity> targets, params BuffId[] debuffs)
		{
			foreach (var target in targets)
			{
				foreach (var debuff in debuffs)
					ApplyBasicDebuff(caster, skill, target, debuff);
			}
		}

		public static void ApplyBasicDebuff(ICombatEntity caster, Skill skill, ICombatEntity target, BuffId debuffId)
		{
			var cooldownId = GetDeactivateBuff(debuffId);
			if (target.IsBuffActive(cooldownId))
				return;

			var stacks = 1;
			if (target.TryGetBuff(debuffId, out var existing))
				stacks = Math.Min(10, existing.OverbuffCounter + 1);

			target.StartBuff(debuffId, skill.Level, 0, BasicDebuffDuration, caster, skill.Id, buff => buff.OverbuffCounter = stacks);
			target.StartBuff(cooldownId, skill.Level, 0, DebuffInternalCooldown, caster, skill.Id);
		}

		public static void ApplyReinforcedDebuffs(ICombatEntity caster, Skill skill, ICombatEntity target)
		{
			PromoteIfMaxed(caster, skill, target, BuffId.Bonemancer_Fist_Debuff, BuffId.Bonemancer_FistReinforce_Debuff);
			PromoteIfMaxed(caster, skill, target, BuffId.Bonemancer_Rib_Debuff, BuffId.Bonemancer_RibReinforce_Debuff);
			PromoteIfMaxed(caster, skill, target, BuffId.Bonemancer_Backbone_Debuff, BuffId.Bonemancer_BackboneReinforce_Debuff);

			if (!caster.IsAbilityActive(AbilityId.Bonemancer9)
				|| !target.IsBuffActive(BuffId.Bonemancer_FistReinforce_Debuff)
				|| !target.IsBuffActive(BuffId.Bonemancer_RibReinforce_Debuff)
				|| !target.IsBuffActive(BuffId.Bonemancer_BackboneReinforce_Debuff))
				return;

			var resonantDuration = TimeSpan.FromSeconds(15);
			target.StartBuff(BuffId.Bonemancer_FistReinforce_Debuff, skill.Level, 0, resonantDuration, caster, skill.Id);
			target.StartBuff(BuffId.Bonemancer_RibReinforce_Debuff, skill.Level, 0, resonantDuration, caster, skill.Id);
			target.StartBuff(BuffId.Bonemancer_BackboneReinforce_Debuff, skill.Level, 0, resonantDuration, caster, skill.Id);
		}

		public static int GetPunctureStacks(ICombatEntity target)
			=> target.TryGetBuff(BuffId.Bonemancer_Fist_Debuff, out var buff) ? Math.Min(10, buff.OverbuffCounter) : 0;

		private static void PromoteIfMaxed(ICombatEntity caster, Skill skill, ICombatEntity target, BuffId basicId, BuffId reinforcedId)
		{
			if (!target.TryGetBuff(basicId, out var basic) || basic.OverbuffCounter < 10)
				return;

			target.StartBuff(reinforcedId, skill.Level, 0, ReinforcedDebuffDuration, caster, skill.Id);
		}

		private static BuffId GetDeactivateBuff(BuffId debuffId)
			=> debuffId switch
			{
				BuffId.Bonemancer_Fist_Debuff => BuffId.Bonemancer_Fist_DeActivate,
				BuffId.Bonemancer_Rib_Debuff => BuffId.Bonemancer_Rib_DeActivate,
				BuffId.Bonemancer_Backbone_Debuff => BuffId.Bonemancer_Backbone_DeActivate,
				_ => throw new ArgumentOutOfRangeException(nameof(debuffId)),
			};
	}
}
