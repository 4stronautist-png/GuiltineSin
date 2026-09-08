using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	/// <summary>
	/// Handles Double Gun Stance as a stance buff instead of an attack.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_DoubleGunStance)]
	public class Bulletmarker_DoubleGunStance : ISelfSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, GuiltineSin.Shared.World.Position originPos, GuiltineSin.Shared.World.Direction dir)
		{
			if (caster is not Character casterCharacter)
				return;

			if (casterCharacter.IsBuffActive(BuffId.DoubleGunStance_Buff))
			{
				Send.ZC_SKILL_READY(caster, skill, ZoneServer.Instance.World.CreateSkillHandle(), originPos, GuiltineSin.Shared.World.Position.Zero);
				Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);
				casterCharacter.RemoveBuff(BuffId.DoubleGunStance_Buff);
				caster.SetAttackState(false);
				return;
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_READY(caster, skill, ZoneServer.Instance.World.CreateSkillHandle(), originPos, GuiltineSin.Shared.World.Position.Zero);
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);
			casterCharacter.StartBuff(BuffId.DoubleGunStance_Buff, skill.Level, 0, TimeSpan.Zero, casterCharacter, skill.Id);
		}
	}
}
