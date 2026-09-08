using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;

/// <summary>
/// Handle for the Frozen, Frozen solid..
/// </summary>
[BuffHandler(BuffId.Sleep_Debuff, BuffId.UC_sleep)]
public class Sleep : BuffHandler, IBuffCombatDefenseAfterCalcHandler
{
	public override void OnActivate(Buff buff, ActivationType activationType)
	{
		buff.Target.AddState(StateType.Sleep);
	}

	public void OnDefenseAfterCalc(Buff buff, ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
	{
		if (skillHitResult.Damage > 0)
			buff.Target.RemoveBuff(buff.Id);
	}

	public override void OnEnd(Buff buff)
	{
		buff.Target.RemoveState(StateType.Sleep);
	}
}
