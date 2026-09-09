using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Network.Helpers;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Bonemancer
{
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneWhip_Swordman)] public class Bonemancer_BoneWhipSwordman : Bonemancer_BoneWhip { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneWhip_Wizard)] public class Bonemancer_BoneWhipWizard : Bonemancer_BoneWhip { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneWhip_Archer)] public class Bonemancer_BoneWhipArcher : Bonemancer_BoneWhip { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneWhip_Cleric)] public class Bonemancer_BoneWhipCleric : Bonemancer_BoneWhip { }

	public class Bonemancer_BoneWhip : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const float PadOffset = 80f;
		private const float PadRange = 80f;
		private const int MaxTargets = 12;
		private static readonly TimeSpan Duration = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(300);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BonemancerSkillHelper.TrySpendSp(caster, skill))
				return;
			skill.IncreaseOverheat();

			var direction = caster.Direction == Direction.Zero
				? BonemancerSkillHelper.GetDirection(caster, originPos, farPos)
				: caster.Direction;
			BonemancerSkillHelper.PrepareGroundSkill(skill, caster, originPos, caster.Position, direction);
			skill.RunFree(BonemancerSkillHelper.FinishRetailCast(skill, caster, TimeSpan.Zero));
			skill.RunFree(this.Channel(skill, caster, direction));
		}

		private async Task Channel(Skill skill, ICombatEntity caster, Direction direction)
		{
			var effectHandle = ForceId.GetNew();
			var padPosition = caster.Position.GetRelative(direction, PadOffset);
			var maxTargets = skill.GetPVPValue(MaxTargets);
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.Bonemancer_BoneWhip, skill, padPosition, direction, 0f, PadRange, effectHandle, PadRange, true);
			var endTime = DateTime.UtcNow + Duration;
			var tick = 0;
			while (DateTime.UtcNow < endTime && !caster.IsDead)
			{
				if (tick % 2 == 1)
					BonemancerSkillHelper.PlayRetailBonePulse(caster);

				var area = new Circle(padPosition, PadRange);
				var targets = BonemancerSkillHelper.GetFixedCountTargets(caster, skill, area, maxTargets);
				var hits = BonemancerSkillHelper.DealHits(caster, skill, targets, SkillModifier.MultiHit(1));
				BonemancerSkillHelper.ApplyBasicDebuffs(caster, skill, hits.Select(hit => hit.Target), BuffId.Bonemancer_Fist_Debuff, BuffId.Bonemancer_Backbone_Debuff);
				tick++;
				await skill.Wait(TickInterval);
			}
			Send.ZC_NORMAL.Unknown_59_SkillVisualEffect(caster, PadName.Bonemancer_BoneWhip, skill, padPosition, direction, 0f, PadRange, effectHandle, PadRange, false);
		}
	}
}
