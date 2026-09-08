using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Scout
{
	/// <summary>
	/// Handler for the Scout skill Double Attack.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Scout_DoubleAttack)]
	public class Scout_DoubleAttackOverride : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill, applying a buff to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="dir"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var duration = TimeSpan.FromSeconds(300);
			var doubleHitChance = 25f + skill.Level * 5f;

			var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
			doubleHitChance *= 1f + SCR_Get_AbilityReinforceRate(skill);

			caster.StartBuff(BuffId.DoubleAttack_Buff, skill.Level, doubleHitChance, duration, caster, skill.Id);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, originPos);
		}
	}
}
