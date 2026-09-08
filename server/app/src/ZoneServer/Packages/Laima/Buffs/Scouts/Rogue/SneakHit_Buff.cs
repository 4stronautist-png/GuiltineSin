using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Scouts.Rogue
{
	/// <summary>
	/// Handler for the SneakHit buff. Increases back attack damage.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.SneakHit_Buff)]
	public class SneakHit_BuffOverride : BuffHandler
	{
		private const float BaseDamageRate = 0.50f;
		private const float DamageRatePerLevel = 0.05f;

		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.SneakHit_Buff)]
		public void OnAttackBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.SneakHit_Buff, out var buff))
				return;

			if (!attacker.IsBehind(target) && !modifier.ForcedBackAttack)
				return;

			var skillLevel = buff.NumArg1;
			var damageIncrease = BaseDamageRate + DamageRatePerLevel * skillLevel;

			skillHitResult.Damage *= 1f + damageIncrease;
		}
	}
}
