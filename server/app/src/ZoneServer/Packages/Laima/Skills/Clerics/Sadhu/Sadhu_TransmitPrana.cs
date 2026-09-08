using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sadhu
{
	/// <summary>
	/// Handler for the Sadhu skill Transmit Prana.
	/// Enchants party members' attacks with Psychokinesis element and increases damage.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sadhu_TransmitPrana)]
	public class Sadhu_TransmitPranaOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const float BuffRange = 300;
		private const int BuffDurationSeconds = 300;
		private const float BaseDamageMultiplierIncrease = 0.10f;
		private const float DamageMultiplierIncreasePerLevel = 0.02f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			var damageMultiplierIncrease = BaseDamageMultiplierIncrease + skill.Level * DamageMultiplierIncreasePerLevel;

			caster.StartBuff(BuffId.TransmitPrana_Buff, skill.Level, damageMultiplierIncrease, TimeSpan.FromSeconds(BuffDurationSeconds), caster);

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
						member.StartBuff(BuffId.TransmitPrana_Buff, skill.Level, damageMultiplierIncrease, TimeSpan.FromSeconds(BuffDurationSeconds), caster);
					}
				}
			}
		}
	}
}
