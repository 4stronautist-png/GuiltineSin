using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Oracle
{
	/// <summary>
	/// Handle for the Foretell Immune buff, which nullifies all
	/// incoming damage while the target is moving.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Foretell_Immune_Buff)]
	public class Oracle_Foretell_Immune_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc_Defense, BuffId.Foretell_Immune_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.IsBuffActive(BuffId.Foretell_Immune_Buff))
				return;

			var isMoving = target.Components.TryGet<MovementComponent>(out var movement) && movement.IsMoving;
			if (isMoving)
				modifier.DamageMultiplier = 0;
		}
	}
}
