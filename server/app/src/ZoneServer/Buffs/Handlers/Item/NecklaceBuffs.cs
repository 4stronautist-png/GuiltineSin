using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for NECK02_123_buff (Necklace of Protection - Darkness II).
	/// Reduces Dark attribute damage taken by 10%.
	/// </summary>
	[BuffHandler(BuffId.NECK02_123_buff)]
	public class NECK02_123_buff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.NECK02_123_buff)]
		public static float OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.NECK02_123_buff, out _))
				return 0;

			var attr = modifier.AttackAttribute == AttributeType.None ? skill.Data.Attribute : modifier.AttackAttribute;
			if (attr == AttributeType.Dark)
				skillHitResult.Damage *= 0.9f;

			return 0;
		}
	}

	/// <summary>
	/// Handle for NECK02_124_buff (Necklace of Protection - Frost II).
	/// Reduces Ice attribute damage taken by 10%.
	/// </summary>
	[BuffHandler(BuffId.NECK02_124_buff)]
	public class NECK02_124_buff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.NECK02_124_buff)]
		public static float OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.NECK02_124_buff, out _))
				return 0;

			var attr = modifier.AttackAttribute == AttributeType.None ? skill.Data.Attribute : modifier.AttackAttribute;
			if (attr == AttributeType.Ice)
				skillHitResult.Damage *= 0.9f;

			return 0;
		}
	}
}
