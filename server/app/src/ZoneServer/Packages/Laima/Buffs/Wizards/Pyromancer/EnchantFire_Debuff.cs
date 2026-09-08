using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Shared.Data.Database;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizard
{
	/// <summary>
	/// Handler for the Enchant Fire debuff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.EnchantFire_Debuff)]
	public class EnchantFire_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.EnchantFire_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.EnchantFire_Debuff, out var buff))
				return;

			if (skill.Data.Attribute != AttributeType.Fire && modifier.AttackAttribute != AttributeType.Fire)
				return;

			var abilityLevel = buff.NumArg2;
			modifier.DamageMultiplier += abilityLevel * 0.1f;
		}
	}
}
