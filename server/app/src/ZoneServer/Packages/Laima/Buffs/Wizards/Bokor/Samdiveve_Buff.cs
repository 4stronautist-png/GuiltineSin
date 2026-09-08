using System;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Bokor
{
	/// <summary>
	/// Handler for the Samdiveve Buff, which increases summon movement speed
	/// and causes summons to apply Decomposition debuff to nearby enemies.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Samdiveve_Buff)]
	public class Samdiveve_BuffOverride : BuffHandler
	{
		private const float BaseBonus = 20;
		private const float BonusPerLevel = 3;
		private const float DecompositionRange = 100;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target is not Summon)
				return;

			buff.SetUpdateTime(1000);

			var mspdBonus = this.GetMspdBonus(buff);
			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, mspdBonus);
		}

		public override void OnEnd(Buff buff)
		{
			if (buff.Target is not Summon)
				return;

			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.Target is not Summon summon)
				return;

			var caster = buff.Caster as ICombatEntity;
			if (caster == null)
				return;

			var enemies = summon.Map.GetAttackableEnemiesInPosition(summon, summon.Position, DecompositionRange)
				.Where(e => !e.IsDead)
				.ToList();

			foreach (var enemy in enemies)
			{
				if (!enemy.IsBuffActive(BuffId.Decomposition_Debuff))
			{
				enemy.StartBuff(BuffId.Decomposition_Debuff, buff.NumArg1, 0f, TimeSpan.FromSeconds(10), summon);
			}
			}
		}

		[CombatCalcModifier(CombatCalcPhase.AfterBonuses, BuffId.Samdiveve_Buff)]
		public void OnAttackAfterBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.Samdiveve_Buff, out var buff))
				return;

			if (attacker is not Summon summon)
				return;

			var damageBonus = buff.NumArg1 * 0.10f;
			skillHitResult.Damage *= 1f + damageBonus;
		}

		private float GetMspdBonus(Buff buff)
		{
			var skillLevel = buff.NumArg1;
			var bonus = BaseBonus + skillLevel * BonusPerLevel;

			var byAbility = 1f;
			if (buff.Caster is ICombatEntity caster && caster.TryGetSkill(buff.SkillId, out var skill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				byAbility += SCR_Get_AbilityReinforceRate(skill);
			}

			return bonus * byAbility;
		}
	}
}
