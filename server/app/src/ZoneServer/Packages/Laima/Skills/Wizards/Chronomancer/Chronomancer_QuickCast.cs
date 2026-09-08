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

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Chronomancer
{
	[Package("laima")]
	[SkillHandler(SkillId.Chronomancer_QuickCast)]
	public class Chronomancer_QuickCastOverride : ISelfSkillHandler
	{
		private const float BuffRange = 300;
		private const int BuffDurationSeconds = 300;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);

			var duration = TimeSpan.FromSeconds(BuffDurationSeconds);

			caster.StartBuff(BuffId.QuickCast_Buff, skill.Level, 0f, duration, caster);

			if (caster is Character character)
			{
				var members = character.GetPartyMembersInRange(BuffRange);
				foreach (var member in members)
				{
					if (member == caster)
						continue;
					member.StartBuff(BuffId.QuickCast_Buff, skill.Level, 0f, duration, caster);
				}
			}
		}
	}
}
