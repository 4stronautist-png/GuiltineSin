using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Zone.Buffs.Base;
using Melia.Zone.Network;
using Melia.Zone.Scripting.ScriptableEvents;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Buffs.Handlers.Scouts.BlitzHunter
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
