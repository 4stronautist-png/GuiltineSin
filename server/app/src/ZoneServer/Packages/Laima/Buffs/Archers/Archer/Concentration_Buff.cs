using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using System;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handler for the Concentration Buff, which increases the target's
	/// hit rate.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Concentration_Buff)]
	public class Concentration_Buff : BuffHandler
	{
		private const float BaseBonus = 0.25f;
		private const float BonusPerLevel = 0.05f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var bonus = this.GetHitRateBonus(buff);

			AddPropertyModifier(buff, buff.Target, PropertyName.HR_RATE_BM, bonus);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.HR_RATE_BM);
		}

		public override void WhileActive(Buff buff)
		{
			var targets = buff.Target.Map.GetAttackableEnemiesInPosition(buff.Target, buff.Target.Position, 100).Where(c => c.IsBuffActiveByKeyword(BuffTag.Cloaking)).ToList();
			foreach (var target in targets)
				target.StopBuffByTag(BuffTag.Cloaking);
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Concentration_Buff)]
		public void OnAttackBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.Concentration_Buff, out var buff))
				return;

			// Archer39 makes hits never miss
			if (buff.Target.TryGetActiveAbilityLevel(AbilityId.Archer39, out _))
				modifier.ForcedHit = true;
		}

		private float GetHitRateBonus(Buff buff)
		{
			var skillLevel = buff.NumArg1;
			var bonus = BaseBonus + skillLevel * BonusPerLevel;

			return bonus;
		}
	}
}
