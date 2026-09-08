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
	/// Handler for the Sacrament Debuff
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Sacrament_Debuff)]
	public class Sacrament_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Sacrament_Debuff)]
		public void OnDefenseBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Sacrament_Debuff, out var buff))
				return;

			if (modifier.AttackAttribute != AttributeType.Holy)
				return;

			modifier.DamageMultiplier += buff.NumArg2;
		}
	}
}
