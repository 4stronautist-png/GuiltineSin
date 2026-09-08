using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Clerics.Paladin
{
	/// <summary>
	/// Handler for the Stone Skin buff.
	/// Reduces physical damage taken.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.StoneSkin_Buff)]
	public class StoneSkin_BuffOverride : BuffHandler
	{
		private const float DamageReductionBase = 0.10f;
		private const float DamageReductionPerLevel = 0.01f;

		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.StoneSkin_Buff)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.StoneSkin_Buff, out var buff))
				return;

			// Only reduce physical damage (not magic)
			if (skill.Data.AttackType == SkillAttackType.Magic)
				return;

			var skillLevel = buff.NumArg1;
			var damageReduction = DamageReductionBase + (skillLevel * DamageReductionPerLevel);

			if (buff.Caster is ICombatEntity casterEntity && casterEntity.TryGetSkill(buff.SkillId, out var buffSkill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				damageReduction *= 1f + SCR_Get_AbilityReinforceRate(buffSkill);
			}

			// Apply damage reduction (Cap at 90%)
			skillHitResult.Damage *= Math.Max(0.1f, (1f - damageReduction));
		}
	}
}
