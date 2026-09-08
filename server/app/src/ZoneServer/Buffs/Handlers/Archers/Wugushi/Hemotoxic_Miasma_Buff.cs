using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Archers.Wugushi;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Short Wide Miasma window that makes Wugushi attacks punish bleeding targets.
	/// </summary>
	[BuffHandler(BuffId.Hemotoxic_Miasma_Buff)]
	public class Hemotoxic_Miasma_Buff : BuffHandler
	{
		private static readonly TimeSpan HealingReductionDuration = TimeSpan.FromSeconds(8);
		private const float HealingReduction = WugushiSkillHelper.HemotoxicMiasmaHealingReductionPercent * 1000f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			Send.ZC_NORMAL.PlayTextEffect(buff.Target, buff.Caster, "SHOW_BUFF_TEXT", (float)BuffId.Hemotoxic_Miasma_Buff, null);
			buff.NotifyUpdate();
		}

		[CombatCalcModifier(CombatCalcPhase.AfterCalc_Attack, BuffId.Hemotoxic_Miasma_Buff)]
		public void OnAttackAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.IsBuffActive(BuffId.Hemotoxic_Miasma_Buff))
				return;

			if (!WugushiSkillHelper.IsWugushiSkill(skill))
				return;

			if (skillHitResult.Damage <= 0)
				return;

			if (!WugushiSkillHelper.IsBleedingEffectActive(target))
				return;

			target.StartBuff(BuffId.WideMiasma_Debuff, skill.Level, HealingReduction, HealingReductionDuration, attacker, skill.Id);
		}
	}
}
