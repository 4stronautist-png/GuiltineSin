using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Cataphract
{
	/// <summary>
	/// Handler for the Cataphract skill Trot.
	/// Toggle skill that increases movement speed while mounted.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Cataphract_Trot)]
	public class Cataphract_TrotOverride : ISelfSkillHandler
	{
		/// <summary>
		/// Handles the Trot skill execution.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (caster is not Character character || !character.IsRiding)
			{
				caster.ServerMessage(Localization.Get("You must be mounted to use this skill."));
				return;
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);

			caster.StartBuff(BuffId.Trot_Buff, skill.Level, 0f, TimeSpan.FromMinutes(5), caster);
		}
	}
}
