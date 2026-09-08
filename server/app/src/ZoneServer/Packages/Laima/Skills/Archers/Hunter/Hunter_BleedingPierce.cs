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
using GuiltineSin.Zone.Pads.Helpers;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;
using Yggdrasil.Extensions;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillUtilHelper;
using GuiltineSin.Zone.World.Actors.Pads;
using GuiltineSin.Zone.Skills.SplashAreas;

namespace GuiltineSin.Zone.Skills.Handlers.Hunter
{
	/// <summary>
	/// Handler for the Hunter skill Bleeding Pierce.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Hunter_BleedingPierce)]
	public class Hunter_BleedingPierceOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.PlaySound("voice_atk_long_cast_f", "voice_war_atk_long_cast");
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.StopSound("voice_atk_long_cast_f", "voice_war_atk_long_cast");
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

			skill.Run(this.HandleSkill(skill, caster, originPos, farPos));

			var targetHandle = target?.Handle ?? 0;
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);

			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var position = caster.Position;
			var padAngle = caster.Direction.DegreeAngle;

			var pad = SkillCreatePad(caster, skill, position, padAngle, PadName.shootpad_BleedingPierce);
			if (pad == null)
				return;

			// Set rectangular area before moving
			var padLength = 170f;
			var padWidth = 30f;
			pad.Direction = caster.Direction;
			pad.SetRectangleRange(caster.Direction, padWidth, padLength);

			// Move pad forward
			var moveRange = 150f;
			var speed = 600f;
			var accel = 50f;
			var destPos = caster.Position.GetRelative(caster.Direction, moveRange);
			await pad.SetDestPosWithDelay(destPos, speed, accel, true, 0f);
		}
	}
}
