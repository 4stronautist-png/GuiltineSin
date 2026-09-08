using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Archers.Wugushi;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Handler for the Wide Miasma stealth buff.
	/// </summary>
	[BuffHandler(BuffId.WideMiasma_Buff)]
	public class WideMiasma_Buff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, WugushiSkillHelper.GetWideMiasmaMoveSpeedBonus(buff.Target));
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);
		}

		[CombatCalcModifier(CombatCalcPhase.AfterCalc_Attack, BuffId.WideMiasma_Buff)]
		public void OnAttackAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (skillHitResult.Damage <= 0)
				return;

			attacker.StopBuff(BuffId.WideMiasma_Buff);
		}
	}
}
