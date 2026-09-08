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
	/// Handler for the Sacrament buff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Sacrament_Buff)]
	public class Sacrament_BuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Sacrament_Buff)]
		public void OnAttackBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.Sacrament_Buff, out var buff))
				return;

			if ((skill.Data.Attribute == AttributeType.None) || (skill.Data.Attribute == AttributeType.Melee) || (skill.Data.Attribute == AttributeType.Magic))
				modifier.AttackAttribute = AttributeType.Holy;
		}
	}
}
