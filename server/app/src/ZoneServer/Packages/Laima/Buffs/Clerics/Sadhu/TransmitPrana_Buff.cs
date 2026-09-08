using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Sadhu
{
	/// <summary>
	/// Handler for the Transmit Prana buff.
	/// Enchants attacks with Psychokinesis element, increases damage,
	/// and grants flat Soul_Atk bonus.
	/// NumArg1: skill level
	/// NumArg2: damage multiplier increase
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.TransmitPrana_Buff)]
	public class TransmitPrana_BuffOverride : BuffHandler
	{
		private const float BaseFlatSoulAtk = 50f;
		private const float FlatSoulAtkPerLevel = 5f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var flatSoulAtk = BaseFlatSoulAtk + FlatSoulAtkPerLevel * buff.NumArg1;
			AddPropertyModifier(buff, buff.Target, PropertyName.Soul_Atk_BM, flatSoulAtk);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.Soul_Atk_BM);
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.TransmitPrana_Buff)]
		public void OnAttackBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.TransmitPrana_Buff, out var buff))
				return;

			var damageMultiplierIncrease = buff.NumArg2;

			if (skill.Data.Attribute == AttributeType.None || skill.Data.Attribute == AttributeType.Melee || skill.Data.Attribute == AttributeType.Magic)
				modifier.AttackAttribute = AttributeType.Soul;

			skillHitResult.Damage *= 1f + damageMultiplierIncrease;
		}
	}
}
