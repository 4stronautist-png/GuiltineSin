using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[BuffHandler(BuffId.Overflux_Debuff)]
	public class Overflux_DebuffOverride : BuffHandler
	{
		private const string StormstrideTrailEffect = "Teleport_SmearDash_Blue_01";
		private const string StormstrideAuraEffect = "BodyAura_Electric_Blue_01";

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Overflux_Debuff)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Overflux_Debuff, out var buff))
				return;

			if (!target.IsAbilityActive(AbilityId.BlitzHunter6))
				return;

			modifier.DamageMultiplier *= 1f - 0.02f * buff.OverbuffCounter;
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;
			if (target == null)
				return;

			Send.ZC_NORMAL.RemoveEffectByName(target, StormstrideTrailEffect, true);
			Send.ZC_NORMAL.RemoveEffectByName(target, StormstrideAuraEffect, true);
		}
	}
}
