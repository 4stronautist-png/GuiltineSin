using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using System;
using System.Threading.Tasks;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[SkillHandler(SkillId.Dragoon_DragonFall)]
	public class Dragoon_DragonFallOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const float MaxLandingDistance = 520f;

		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.ClearTargets();
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var targetPos = DragoonSkillHelper.ClampTargetPosition(caster, originPos, farPos, MaxLandingDistance);
			this.FinishDragonFall(skill, caster, originPos, targetPos);
		}

		private void FinishDragonFall(Skill skill, ICombatEntity caster, Position originPos, Position targetPos)
		{
			if (!DragoonSkillHelper.StartGroundSkill(skill, caster, originPos, targetPos))
				return;

			skill.Run(this.FallAndAttack(caster, skill, targetPos));
		}

		private async Task FallAndAttack(ICombatEntity caster, Skill skill, Position targetPos)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(650));

			var landingPos = targetPos;
			caster.Position = landingPos;
			Send.ZC_SET_POS(caster, landingPos);
			Send.ZC_NORMAL.PlayEffectAtPosition(caster, "skl_DragonFall_cast", landingPos, 2f, ForceId.GetNew(), 2000);

			await DragoonSkillHelper.AttackCircle(caster, skill, landingPos, 190, 50, 6, modify: DragoonSkillHelper.ApplySlowOrHoldBonus);
		}
	}
}
