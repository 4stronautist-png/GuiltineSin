using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using Yggdrasil.Logging;

namespace GuiltineSin.Zone.Skills.Handlers.Cryomancer
{
	/// <summary>
	/// Handler for the Cryomancer skill Subzero Shield.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Cryomancer_SubzeroShield)]
	public class Cryomancer_SubzeroShieldOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int BuffRange = 300;
		private const int BuffDurationSeconds = 300;
		private const int FreezeDurationMilliseconds = 500;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();

			Send.ZC_SKILL_READY(caster, skill, skillHandle, caster.Position, caster.Position);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, caster.Handle, caster.Position, caster.Direction, caster.Position);

			Send.ZC_SYNC_START(caster, skillHandle, 1);

			var freezeDuration = FreezeDurationMilliseconds;

			if (caster.TryGetActiveAbility(AbilityId.Cryomancer9, out var ability))
				freezeDuration += 500 * ability.Level;

			if (caster.TryGetActiveAbility(AbilityId.Cryomancer7, out var abilCryomancer7))
				freezeDuration += (int)(500 * abilCryomancer7.Level);

			caster.StartBuff(BuffId.Subzero_Buff, skill.Level, freezeDuration, TimeSpan.FromSeconds(BuffDurationSeconds), caster);

			// Buff party members
			if (caster is Character character)
			{
				var party = character.Connection.Party;
				if (party != null)
				{
					var members = caster.Map.GetPartyMembersInRange(character, BuffRange, true);
					foreach (var member in members)
					{
						if (member == caster)
							continue;
						member.StartBuff(BuffId.Subzero_Buff, skill.Level, freezeDuration, TimeSpan.FromSeconds(BuffDurationSeconds), caster);
					}
				}
			}

			Send.ZC_SYNC_END(caster, skillHandle, 0);
			Send.ZC_SYNC_EXEC_BY_SKILL_TIME(caster, skillHandle, TimeSpan.FromMilliseconds(400));
			Send.ZC_SKILL_RANGE_FAN(caster, originPos, caster.Direction, 0, 0.1396263f);
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster);

			Send.ZC_NORMAL.Skill_45(caster);
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
		}
	}
}
