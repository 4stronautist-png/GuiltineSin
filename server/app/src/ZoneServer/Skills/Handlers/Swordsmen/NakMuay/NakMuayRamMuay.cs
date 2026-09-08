using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.NakMuay
{
	/// <summary>
	/// Handler for the NakMuay skill Ram Muay.
	/// </summary>
	[SkillHandler(SkillId.NakMuay_RamMuay)]
	public class NakMuayRamMuay : ISelfSkillHandler
	{
		/// <summary>
		/// Handles skill, applying the Ram Muay buff to the caster.
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

			if (caster is Character character)
			{
				Send.ZC_STANCE_CHANGE(character);

				if (caster.IsBuffActive(BuffId.RamMuay_Buff))
				{
					caster.StopBuff(BuffId.RamMuay_Buff);
				}
				else
					caster.StartBuff(BuffId.RamMuay_Buff, skill.Level, 0, TimeSpan.Zero, caster, skill.Id);

				Send.ZC_STANCE_CHANGE(character);
			}

			// Notify client about the stance change and skill animation
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster, null);

			skill.IncreaseOverheat();
			caster.SetAttackState(true);
		}
	}
}
