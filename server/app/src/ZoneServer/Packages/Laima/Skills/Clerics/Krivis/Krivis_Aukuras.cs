using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Kriwi
{
	/// <summary>
	/// Handler for the Kriwi skill Aukuras.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Kriwi_Aukuras)]
	public class Krivis_AukurasOverride : IGroundSkillHandler, IDynamicCasted
	{

		/// <summary>
		/// Handles skill
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

			skill.Run(this.HandleSkill(caster, skill));
		}
		private async Task HandleSkill(ICombatEntity caster, Skill skill)
		{
			SkillRemovePad(caster, skill);
			await skill.Wait(TimeSpan.FromMilliseconds(300));

			if (caster.TryGetActiveAbilityLevel(AbilityId.Kriwi14, out var abilityLevel))
				SkillCreatePad(caster, skill, caster.Position, abilityLevel, PadName.Cleric_New_Aukuras);
			else
				SkillCreatePad(caster, skill, caster.Position, 0, PadName.Cleric_New_Aukuras);
		}
	}
}
