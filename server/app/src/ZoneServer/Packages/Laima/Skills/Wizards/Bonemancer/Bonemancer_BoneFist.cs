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
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneFist_Swordman)] public class Bonemancer_BoneFistSwordman : Bonemancer_BoneFist { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneFist_Wizard)] public class Bonemancer_BoneFistWizard : Bonemancer_BoneFist { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneFist_Archer)] public class Bonemancer_BoneFistArcher : Bonemancer_BoneFist { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneFist_Cleric)] public class Bonemancer_BoneFistCleric : Bonemancer_BoneFist { }

	public class Bonemancer_BoneFist : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const float DamageAreaRange = 130f;
		private const float DamageAreaHalfWidth = 65f;
		private const int MaxTargets = 5;
		private static readonly TimeSpan InitialDamageDelay = TimeSpan.FromMilliseconds(450);
		private static readonly TimeSpan HitInterval = TimeSpan.FromMilliseconds(100);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var hasAgglomeration = caster.IsAbilityActive(AbilityId.Bonemancer7);
			if (!BonemancerSkillHelper.TrySpendSp(caster, skill, hasAgglomeration ? 1.3f : 1f))
				return;

			skill.IncreaseOverheat();
			var direction = caster.Direction == Direction.Zero
				? BonemancerSkillHelper.GetDirection(caster, originPos, farPos)
				: caster.Direction;
			BonemancerSkillHelper.PrepareGroundSkill(skill, caster, originPos, caster.Position, direction);

			skill.RunFree(this.ApplyDamage(skill, caster, direction, hasAgglomeration));
			skill.RunFree(BonemancerSkillHelper.FinishRetailCast(skill, caster, TimeSpan.FromMilliseconds(1000), true));
		}

		private async Task ApplyDamage(Skill skill, ICombatEntity caster, Direction direction, bool hasAgglomeration)
		{
			await skill.Wait(InitialDamageDelay);

			var range = skill.Properties.GetFloat(PropertyName.MaxR);
			if (range <= 0)
				range = DamageAreaRange;

			var area = new Square(caster.Position, direction, range, DamageAreaHalfWidth);
			var maxTargets = hasAgglomeration ? 1 : skill.GetPVPValue(MaxTargets);
			var hitTargets = BonemancerSkillHelper.GetFixedCountTargets(caster, skill, area, maxTargets);
			var hitCount = Math.Max(1, skill.Properties.MultiHitCount);
			for (var hitIndex = 0; hitIndex < hitCount && hitTargets.Count > 0 && !caster.IsDead; ++hitIndex)
			{
				if (hitIndex == 0 || hitIndex + 1 == hitCount)
					BonemancerSkillHelper.PlayRetailBonePulse(caster);

				var modifier = SkillModifier.MultiHit(1);
				if (hasAgglomeration)
					modifier.FinalDamageMultiplier *= 1.5f;

				var hits = BonemancerSkillHelper.DealHits(caster, skill, hitTargets, modifier);
				if (hitIndex == 0)
					BonemancerSkillHelper.ApplyBasicDebuffs(caster, skill, hits.Select(hit => hit.Target), BuffId.Bonemancer_Fist_Debuff);

				if (hitIndex + 1 < hitCount)
					await skill.Wait(HitInterval);
			}
		}
	}
}
