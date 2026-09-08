using System;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Priest
{
	/// <summary>
	/// Handler for the Priest skill Revive.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Priest_Revive)]
	public class Priest_ReviveOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const float BuffRange = 250f;
		private const int BuffDurationSeconds = 300;

		/// <summary>
		/// Handles the execution of the Revive skill.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			var buffDuration = TimeSpan.FromSeconds(BuffDurationSeconds);
			var healAmount = this.CalculateHealAmount(caster, caster, skill);

			caster.StartBuff(BuffId.Cleric_Revival_Buff, skill.Level, healAmount, buffDuration, caster);

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

						healAmount = this.CalculateHealAmount(caster, member, skill);
						member.StartBuff(BuffId.Cleric_Revival_Buff, skill.Level, healAmount, buffDuration, caster);
					}
				}
			}
		}

		/// <summary>
		/// Calculates the heal amount for the target.
		/// </summary>
		private float CalculateHealAmount(ICombatEntity caster, ICombatEntity target, Skill skill)
		{
			var SCR_CalculateHeal = ScriptableFunctions.Combat.Get("SCR_CalculateHeal");
			var modifier = new SkillModifier();
			var skillHitResult = new SkillHitResult();
			var healAmount = SCR_CalculateHeal(caster, target, skill, modifier, skillHitResult);

			return healAmount;
		}
	}
}
