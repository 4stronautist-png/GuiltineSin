using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Hoplite
{
	/// <summary>
	/// Handle for the Spear Lunge debuff.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[BuffHandler(BuffId.SpearLunge_Debuff)]
	public class SpearLunge_Debuff : BuffHandler
	{
		/// <summary>
		/// Applies the debuff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.SpearLunge_Debuff)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.SpearLunge_Debuff, out var buff))
				return;

			if (attacker.Components.TryGet<InventoryComponent>(out var inventory))
			{
				var rightHand = inventory.GetItem(EquipSlot.RightHand);
				var type = rightHand?.Data.EquipType1 ?? EquipType.None;

				if (type == EquipType.Spear || type == EquipType.THSpear)
					modifier.DamageMultiplier += 0.5f;
			}
		}
	}
}
