using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillUtilHelper;
using GuiltineSin.Zone.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Scripting;

namespace GuiltineSin.Zone.Skills.Handlers.Pyromancer
{
	/// <summary>
	/// Handler for the Pyromancer skill Enchant Fire.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Pyromancer_EnchantFire)]
	public class Pyromancer_EnchantFireOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const float BuffRange = 300;
		private const int BuffDurationSeconds = 300;
		private const float DamageMultiplierIncreasePerLevel = 0.02f;

		/// <summary>
		/// Handle Skill Behavior
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		/// <param name="targets"></param>
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

			var damageMultiplierIncrease = skill.Level * DamageMultiplierIncreasePerLevel;

			var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
			damageMultiplierIncrease *= 1f + SCR_Get_AbilityReinforceRate(skill);

			caster.StartBuff(BuffId.EnchantFire_Buff, skill.Level, damageMultiplierIncrease, TimeSpan.FromSeconds(BuffDurationSeconds), caster);

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
						member.StartBuff(BuffId.EnchantFire_Buff, skill.Level, damageMultiplierIncrease, TimeSpan.FromSeconds(BuffDurationSeconds), caster);
					}
				}
			}

			// Debuff enemies with ability
			if (caster.TryGetActiveAbility(AbilityId.Pyromancer6, out var ability))
			{
				var pad = SkillCreatePad(caster, skill, caster.Position, 0, PadName.Wizard_New_EnchantFire);
				if (pad == null)
					return;
				pad.Trigger.MaxConcurrentUseCount = ability.Level;
				pad.NumArg2 = ability.Level;
			}
		}
	}
}
