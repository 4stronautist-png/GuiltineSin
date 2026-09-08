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
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Sapper
{
	/// <summary>
	/// Handler for the Sapper skill Spring Trap.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sapper_SpringTrap)]
	public class Sapper_SpringTrapOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int CastDelayMs = 800;
		private const float SpawnDistance = 15f;

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
		}

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
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.HandleSkill(caster, skill, originPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(CastDelayMs));

			var targetPos = caster.Position.GetRelative(caster.Direction, distance: SpawnDistance);
			SkillCreatePad(caster, skill, targetPos, (float)caster.Direction.DegreeAngle, PadName.Sapper_SpringTrap);
		}
	}
}
