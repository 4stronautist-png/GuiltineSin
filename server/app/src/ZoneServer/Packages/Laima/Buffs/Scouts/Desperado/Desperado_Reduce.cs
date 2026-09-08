using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Desperado
{
	/// <summary>
	/// Last Man Standing PvP incoming damage reduction.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Desperado_Reduce)]
	public class Desperado_ReduceOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.Desperado_Reduce)]
		public void OnBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.IsBuffActive(BuffId.Desperado_Reduce))
				return;

			modifier.DamageMultiplier *= 0.5f;
		}
	}
}
