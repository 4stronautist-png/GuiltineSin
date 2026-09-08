using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[BuffHandler(BuffId.Serpentine_Debuff)]
	public class Serpentine_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Serpentine_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Serpentine_Debuff, out var buff))
				return;

			if (buff.Caster?.Handle != attacker.Handle)
				return;

			var bonus = buff.NumArg2 > 0 ? buff.NumArg2 / 100f : 0.20f;
			modifier.FinalDamageMultiplier *= 1f + bonus;
		}
	}

	[Package("laima")]
	[BuffHandler(BuffId.DragonFear_Debuff)]
	public class HatredForDragons_BuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragonFear_Debuff)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.DragonFear_Debuff, out var buff))
				return;

			var level = Math.Max(1, (int)buff.NumArg1);
			if (target is Mob { Race: RaceType.Widling })
				modifier.FinalDamageMultiplier *= 1f + level * 0.07f;

			if (target is Mob { Rank: MonsterRank.Boss })
				modifier.FinalDamageMultiplier *= 1f + level * 0.025f;

			if (DragoonSkillHelper.IsDragoonSkill(skill) && RandomProvider.Get().Next(100) < level)
				target.StartBuff(BuffId.DragonFear_Slow_Debuff, level, 0, TimeSpan.FromSeconds(8), attacker, skill.Id);
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragonFear_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.IsBuffActive(BuffId.DragonFear_Debuff))
				return;

			if (attacker is Mob && attacker.Race != RaceType.Widling && attacker.Race != RaceType.Velnias)
				modifier.FinalDamageMultiplier *= 1.15f;
		}
	}

	[Package("laima")]
	[BuffHandler(BuffId.DragonFear_Slow_Debuff)]
	public class DragonWound_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragonFear_Slow_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.IsBuffActive(BuffId.DragonFear_Slow_Debuff))
				return;

			modifier.DefensePenetrationRate += 0.10f;
		}
	}
}
