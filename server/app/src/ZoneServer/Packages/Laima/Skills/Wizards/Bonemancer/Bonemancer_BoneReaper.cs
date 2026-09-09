using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Wizards.Bonemancer
{
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReaper_Swordman)] public class Bonemancer_BoneReaperSwordman : Bonemancer_BoneReaper { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReaper_Wizard)] public class Bonemancer_BoneReaperWizard : Bonemancer_BoneReaper { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReaper_Archer)] public class Bonemancer_BoneReaperArcher : Bonemancer_BoneReaper { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneReaper_Cleric)] public class Bonemancer_BoneReaperCleric : Bonemancer_BoneReaper { }

	public class Bonemancer_BoneReaper : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const float DamageAreaOffset = -10f;
		private const float DamageAreaRange = 100f;
		private const float DamageAreaAngle = 90f;
		private const int MaxTargets = 8;
		private const int HitsPerWave = 3;
		private static readonly TimeSpan InitialDamageDelay = TimeSpan.FromMilliseconds(300);
		private static readonly TimeSpan WaveInterval = TimeSpan.FromMilliseconds(300);

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
			skill.RunFree(this.ApplyDamageWaves(skill, caster, direction));
			skill.RunFree(BonemancerSkillHelper.FinishRetailCast(skill, caster, TimeSpan.FromMilliseconds(600)));
		}

		private async Task ApplyDamageWaves(Skill skill, ICombatEntity caster, Direction direction)
		{
			await skill.Wait(InitialDamageDelay);
			this.ApplyDamageWave(skill, caster, direction);
			await skill.Wait(WaveInterval);
			this.ApplyDamageWave(skill, caster, direction);
		}

		private void ApplyDamageWave(Skill skill, ICombatEntity caster, Direction direction)
		{
			var areaOrigin = caster.Position.GetRelative(direction, DamageAreaOffset);
			var area = new Fan(areaOrigin, direction, DamageAreaRange, DamageAreaAngle);
			var maxTargets = skill.GetPVPValue(MaxTargets);
			var targets = BonemancerSkillHelper.GetFixedCountTargets(caster, skill, area, maxTargets);
			var hits = BonemancerSkillHelper.DealHits(caster, skill, targets, SkillModifier.MultiHit(HitsPerWave));
			BonemancerSkillHelper.ApplyBasicDebuffs(caster, skill, hits.Select(hit => hit.Target), BuffId.Bonemancer_Fist_Debuff, BuffId.Bonemancer_Rib_Debuff);
		}
	}
}
