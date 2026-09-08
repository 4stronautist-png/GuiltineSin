using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using GuiltineSin.Zone.Skills.Helpers;

namespace GuiltineSin.Zone.Skills.Handlers.Mon
{
	[SkillHandler(SkillId.Mon_blindlem_Skill_1)]
	public class Mon_blindlem_Skill_1 : ITargetSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.TurnTowards(target);
			caster.SetAttackState(true);

			var originPos = caster.Position;
			var farPos = originPos.GetNearestPositionWithinDistance(target.Position, skill.Properties[PropertyName.MaxR]);
			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);

			skill.Run(this.HandleSkill(caster, target, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, ICombatEntity target, Skill skill, Position originPos, Position farPos)
		{
			var targetPos = farPos;
			caster.SetTargets(SkillSelectEnemiesInCircle(caster, targetPos, 60f, 5));
			await MissileFall(caster, skill, targetPos, new MissileConfig
			{
				Effect = new EffectConfig("I_force001_yellow", 1.5f),
				EndEffect = new EffectConfig("F_explosion008_yellow", 1f),
				DotEffect = EffectConfig.None,
				Range = 20f,
				DelayTime = 0f,
				FlyTime = 1.2f,
				Height = 300f,
				Easing = 2f,
				HitTime = 500f,
				HitCount = 1,
				HitStartFix = 0f,
				StartEasing = 0f,
				GroundEffect = new EffectConfig("None", 0.5f),
			});
		}
	}

}
