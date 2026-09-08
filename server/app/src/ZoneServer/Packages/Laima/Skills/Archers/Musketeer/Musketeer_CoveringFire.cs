using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.Musketeer
{
	/// <summary>
	/// Handler for the Musketeer skill Covering Fire.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Musketeer_CoveringFire)]
	public class Musketeer_CoveringFireOverride : IGroundSkillHandler, IDynamicCasted
	{
		TimeSpan DamageDelay = TimeSpan.FromMilliseconds(100);

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

			var targetHandle = target?.Handle ?? 0;
			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, targetHandle, originPos, originPos.GetDirection(farPos), Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, ForceId.GetNew(), null);

			skill.Run(this.HandleSkill(caster, skill, targetPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position targetPos)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(5));

			var splashParam = skill.GetSplashParameters(caster, targetPos, targetPos, length: 50, width: 50, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);

			var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);
			var results = new List<SkillHitResult>();
			var hitTargets = new List<ICombatEntity>();

			foreach (var target in targets.LimitBySDR(caster, skill))
			{
				var modifier = SkillModifier.MultiHit(skill.Data.MultiHitCount);

				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				target.TakeDamage(skillHitResult.Damage, caster);
				results.Add(skillHitResult);

				var hit = new HitInfo(caster, target, skill, skillHitResult);
				hit.ResultType = skillHitResult.Result;
				hit.AniTime = DamageDelay;

				if (skillHitResult.Damage > 0)
				{
					hitTargets.Add(target);
				}

				Send.ZC_HIT_INFO(hit);
			}

			await skill.Wait(TimeSpan.FromMilliseconds(1000));
			Send.ZC_NORMAL.SkillCancelCancel(caster, skill.Id);
		}
	}
}
