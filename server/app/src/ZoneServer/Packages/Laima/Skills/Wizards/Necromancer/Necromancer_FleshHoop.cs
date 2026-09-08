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
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Necromancer
{
	/// <summary>
	/// Handler for the Necromancer skill Flesh Hoop.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Necromancer_FleshHoop)]
	public class Necromancer_FleshHoopOverride : IForceGroundSkillHandler
	{
		protected TimeSpan DamageDelay { get; } = TimeSpan.FromMilliseconds(300);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_FORCE_GROUND(caster, skill, farPos, forceId, null);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			var targetPos = originPos.GetRelative(farPos);
			SkillCreatePad(caster, skill, targetPos, 0f, PadName.Necromancer_FleshHoop_abil);
			await skill.Wait(TimeSpan.FromMilliseconds(400));
			if (caster is not Character character)
				return;

			var royal = caster.IsAbilityActive(AbilityId.Necromancer25);
			var sacrificeCount = 1;
			if (NecromancerSkillHelper.SacrificeRandomSkeleton(character, sacrificeCount) < sacrificeCount)
				return;

			var shieldRate = 4f + Math.Max(1, skill.Level);
			var duration = TimeSpan.FromSeconds(8);
			if (!royal)
			{
				caster.StartBuff(BuffId.FleshHoop_Buff, shieldRate, 0f, duration, caster, skill.Id);
				return;
			}

			var members = character.HasParty ? character.Map.GetPartyMembers(character) : [character];
			foreach (var member in members.Where(member => member.Map == character.Map))
				member.StartBuff(BuffId.FleshHoop_Buff, shieldRate * 0.5f, 0f, duration * 0.5, caster, skill.Id);
		}
	}
}
