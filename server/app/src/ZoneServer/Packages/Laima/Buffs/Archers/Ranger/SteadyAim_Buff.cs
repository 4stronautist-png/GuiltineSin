using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Steady Aim, Increases damage when Two-Handed Bow or Crossbow is equipped..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.SteadyAim_Buff)]
	public class SteadyAim_Buff : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.SteadyAim_Buff)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.SteadyAim_Buff, out var buff))
				return;

			if (attacker.TryGetEquipItem(EquipSlot.RightHand, out var equipItem)
				&& (equipItem.Data.EquipType1 == EquipType.Bow
				|| equipItem.Data.EquipType1 == EquipType.THBow))
			{
				modifier.CritDamageMultiplier += buff.NumArg2;
			}
		}
	}
}
