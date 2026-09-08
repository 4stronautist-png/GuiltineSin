using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Scouts.Rogue
{
	/// <summary>
	/// Handler for the Lachrymator debuff. Resets all hate on the
	/// target mob and prevents hate gain. Removed when hit.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Lachrymator_Debuff)]
	public class Lachrymator_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.AddState(StateType.Held);

			if (buff.Target.Components.TryGet<AiComponent>(out var ai))
				ai.Script.QueueEventAlert(new HateResetAlert());
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.RemoveState(StateType.Held);
		}

		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Lachrymator_Debuff)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Lachrymator_Debuff, out var buff))
				return;

			target.StopBuff(BuffId.Lachrymator_Debuff);
		}
	}
}
