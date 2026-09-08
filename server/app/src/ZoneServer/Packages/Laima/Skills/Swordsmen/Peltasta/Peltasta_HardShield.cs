using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Peltasta
{
	/// <summary>
	/// Handler for the Peltasta Skill Hard Shield
	/// </summary>
	/// <remarks>
	/// </remarks>
	[Package("laima")]
	[SkillHandler(SkillId.Peltasta_HardShield)]
	public class Peltasta_HardShieldOverride : IGroundSkillHandler
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

			/// TODO: This needs to be immediately removed if your shield
			///   is removed, probably need an equipment change hook.

			var duration = TimeSpan.FromSeconds(300);
			caster.StartBuff(BuffId.HardShield_Buff, skill.Level, 0, duration, caster, skill.Id);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, originPos);
		}
	}
}
