using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Scouts.Desperado;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Desperado
{
	/// <summary>
	/// Bad Guy: Death Approaching final damage bonus.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Desperado20_Buff)]
	public class Desperado20_BuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Desperado20_Buff)]
		public void OnBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!DesperadoSkillHelper.IsDesperadoDamageSkill(skill.Id))
				return;

			if (!attacker.TryGetBuff(BuffId.Desperado20_Buff, out var buff))
				return;

			modifier.FinalDamageMultiplier *= 1f + 0.03f * buff.OverbuffCounter;
		}
	}
}
