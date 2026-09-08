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
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Rogue
{
	/// <summary>
	/// Handler for the Rogue skill Burrow.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Rogue_Burrow)]
	public class Rogue_BurrowOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var targetHandle = target?.Handle ?? 0;
			var buffActive = caster.IsBuffActive(BuffId.Burrow_Rogue);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, buffActive ? 1 : 0, targetHandle, caster.Position, caster.Direction, Position.Zero);

			skill.Run(this.HandleSkill(skill, caster));

			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster)
		{
			if (caster.IsBuffActive(BuffId.Burrow_Rogue))
			{
				caster.StopBuff(BuffId.Burrow_Rogue);
			}
			else
			{
				SkillResetCooldown(skill, caster);
				await skill.Wait(TimeSpan.FromMilliseconds(600));
				caster.StartBuff(BuffId.Burrow_Rogue, skill.Level, 0, TimeSpan.FromSeconds(60), caster, skill.Id);
			}
		}
	}
}
