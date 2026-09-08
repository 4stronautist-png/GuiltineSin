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
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillUtilHelper;
using Yggdrasil.Extensions;
using GuiltineSin.Zone.Skills.Helpers;

namespace GuiltineSin.Zone.Skills.Handlers.Hunter
{
	/// <summary>
	/// Handler for the Hunter skill Bolas.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Hunter_Bolas)]
	public class Hunter_BolasOverride : IGroundSkillHandler, IDynamicCasted
	{

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!skill.Vars.TryGet<Position>("GuiltineSin.ToolGroundPos", out var targetPos))
			{
				caster.ServerMessage(Localization.Get("No target location specified."));
				return;
			}
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.HandleSkill(skill, caster, targetPos));
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position targetPos)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(450));
			await MissilePadThrow(skill, caster, targetPos, new MissileConfig
			{
				Effect = new EffectConfig("I_bolas_mesh#Bip01 R Hand", 1.125f),
				EndEffect = new EffectConfig("F_archer_explosiontrap_shot_smoke", 0.5f),
				DotEffect = EffectConfig.None,
				Range = 0f,
				FlyTime = 0.3f,
				DelayTime = 0f,
				Gravity = -10f,
				Speed = 1f,
				HitTime = 1000f,
				HitCount = 0,
				GroundEffect = EffectConfig.None,
				GroundDelay = 0f,
				EffectMoveDelay = 0f,
			}, 0f, PadName.Shootpad_Bolas);
			
			if (caster.TryGetActiveGroundCompanion(out var companion))
			{
				var enemies = caster.Map.GetAttackableEnemiesInPosition(caster, targetPos, 40f);

				foreach (var enemy in enemies)
				{
					companion.InsertHate(enemy);
				}
			}
		}
	}
}
