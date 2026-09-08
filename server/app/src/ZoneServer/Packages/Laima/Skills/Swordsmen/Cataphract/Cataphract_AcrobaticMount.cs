using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Cataphract
{
	/// <summary>
	/// Handler for the Cataphract skill Acrobatic Mount.
	/// Passive skill that grants bonuses after mounted dash.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Cataphract_AcrobaticMount)]
	public class Cataphract_AcrobaticMountOverride : ISelfSkillHandler, ISkillOnBuffStartHandler
	{
		/// <summary>
		/// Handles the Acrobatic Mount skill execution.
		/// This is now a passive skill - using it just shows a message.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			caster.ServerMessage(Localization.Get("Acrobatic Mount is a passive skill. Dash while mounted to activate its effects."));
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);
		}

		/// <summary>
		/// Called when any buff starts on the character.
		/// Applies Acrobatic Mount buff when DashRun starts while mounted.
		/// </summary>
		public void OnBuffStart(Skill skill, ICombatEntity target, Buff buff)
		{
			if (buff.Id != BuffId.DashRun)
				return;

			if (!target.IsRiding())
				return;

			target.StartBuff(BuffId.AcrobaticMount_Buff, skill.Level, 0, TimeSpan.FromSeconds(5), target, skill.Id);
		}
	}
}
