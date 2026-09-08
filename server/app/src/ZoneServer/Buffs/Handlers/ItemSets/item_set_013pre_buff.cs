using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.ItemSets
{
	/// <summary>
	/// Handler for the Earth Armor Set 4-piece pre-buff.
	/// When hit by Earth damage, triggers item_set_013_buff.
	/// </summary>
	[BuffHandler(BuffId.item_set_013pre_buff)]
	public class item_set_013pre_buff : BuffHandler
	{
		/// <summary>
		/// Duration of the triggered buff in milliseconds.
		/// </summary>
		private const int BuffDuration = 10000;

		/// <summary>
		/// Called after defense damage is calculated (when taking damage).
		/// Triggers item_set_013_buff when hit by Earth damage.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.item_set_013pre_buff)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.item_set_013pre_buff, out var buff))
				return;

			// Only characters can benefit
			if (target is not Character)
				return;

			// Only trigger on Earth attribute attacks
			var attribute = modifier.AttackAttribute != AttributeType.None
				? modifier.AttackAttribute
				: skill.Data.Attribute;

			if (attribute != AttributeType.Earth)
				return;

			// Apply the buff (overbuff counter tracks damage)
			target.StartBuff(BuffId.item_set_013_buff, 1, 0, TimeSpan.FromMilliseconds(BuffDuration), target);
		}
	}
}
