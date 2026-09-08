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
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sadhu
{
	/// <summary>
	/// Handler for the Sadhu skill Astral Body Smite.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sadhu_AstralBodySmite)]
	public class Sadhu_AstralBodySmiteOverride : IGroundSkillHandler
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
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			var targetPos = originPos.GetRelative(farPos, distance: 50f);
			var value = skill.GetPVPValue(5);
			caster.SetTargets(SkillSelectEnemiesInCircle(caster, targetPos, 60f, value));
			var targets = caster.GetTargets();
			caster.StartBuff(BuffId.Sadhu_Soul_Pre_Buff, 1f, 0f, TimeSpan.Zero, caster);
			caster.StartBuff(BuffId.Sadhu_Soul_Buff, 1f, 0f, TimeSpan.FromMilliseconds(60000f), caster);
			await skill.Wait(TimeSpan.FromMilliseconds(500));
			SkillTargetDamage(skill, caster, targets, 1f);
			await skill.Wait(TimeSpan.FromMilliseconds(100));
			targetPos = originPos.GetRelative(farPos, distance: 50f);
			SkillCreatePad(caster, skill, targetPos, 0f, PadName.Sadhu_AstralBodySmite);
			SkillTargetEffects(skill, caster, targets, "F_wizard_compulsionlink_shot_explosion_blue", 0.8f, false);
		}
	}
}
