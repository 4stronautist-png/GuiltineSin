using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Scouts.Rogue
{
	/// <summary>
	/// Handler for the KnifeThrowing_Debuff (Bull's-eye).
	/// Increases damage taken from critical attacks.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.KnifeThrowing_Debuff)]
	public class KnifeThrowing_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.KnifeThrowing_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.KnifeThrowing_Debuff, out var buff))
				return;

			if (skillHitResult.Result != HitResultType.Crit)
				return;

			var skillLevel = (int)buff.NumArg1;
			var bonusDamage = 0.20f + (skillLevel * 0.02f);

			skillHitResult.Damage *= (1 + bonusDamage);
		}
	}
}
