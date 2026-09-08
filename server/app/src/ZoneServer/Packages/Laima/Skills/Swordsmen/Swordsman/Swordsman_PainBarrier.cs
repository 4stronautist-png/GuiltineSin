using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.HandlersOverrides.Swordsmen.Swordsman
{
	/// <summary>
	/// Handler for the Swordman skill Pain Barrier.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Swordman_PainBarrier)]
	public class Swordman_PainBarrierOverride : ISelfSkillHandler
	{
		/// <summary>
		/// Handles skill, applying the buffs to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="dir"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var target = caster;

			var mainDuration = TimeSpan.FromSeconds(10 + skill.Level * 4);
			var immunityDuration = TimeSpan.FromSeconds(3);

			target.StartBuff(BuffId.PainBarrier_Buff, skill.Level, 0, mainDuration, caster);
			target.StartBuff(BuffId.PainBarrierImmune_Buff, skill.Level, 0, immunityDuration, caster);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, target);
		}
	}
}
