using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melia.Shared.Data.Database;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Wizards.Bonemancer
{
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneStorm_Swordman)] public class Bonemancer_BoneStormSwordman : Bonemancer_BoneStorm { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneStorm_Wizard)] public class Bonemancer_BoneStormWizard : Bonemancer_BoneStorm { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneStorm_Archer)] public class Bonemancer_BoneStormArcher : Bonemancer_BoneStorm { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneStorm_Cleric)] public class Bonemancer_BoneStormCleric : Bonemancer_BoneStorm { }

	public class Bonemancer_BoneStorm : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const float DamageAreaRange = 90f;
		private const int MaxTargets = 15;
		private const int HitCountPerTick = 2;
		private const int TickCount = 10;
		private static readonly TimeSpan Duration = TimeSpan.FromMilliseconds(5100);
		private static readonly TimeSpan InitialDamageDelay = TimeSpan.FromMilliseconds(500);
		private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(500);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BonemancerSkillHelper.TrySpendSp(caster, skill))
				return;
			skill.IncreaseOverheat();

			var direction = BonemancerSkillHelper.GetDirection(caster, originPos, farPos);
			BonemancerSkillHelper.PrepareGroundSkill(skill, caster, originPos, caster.Position, direction);
			caster.StartBuff(BuffId.BoneStorm_Buff, skill.Level, 0, Duration, caster, skill.Id);
			skill.RunFree(BonemancerSkillHelper.FinishRetailCast(skill, caster, TimeSpan.FromMilliseconds(500)));
			skill.RunFree(this.Channel(skill, caster));
		}

		private async Task Channel(Skill skill, ICombatEntity caster)
		{
			await skill.Wait(InitialDamageDelay);

			var maxTargets = skill.GetPVPValue(MaxTargets);
			for (var tick = 0; tick < TickCount && !caster.IsDead; ++tick)
			{
				if (tick % 2 == 0 || tick + 1 == TickCount)
					BonemancerSkillHelper.PlayRetailBonePulse(caster);

				var area = new Circle(caster.Position, DamageAreaRange);
				var targets = BonemancerSkillHelper.GetFixedCountTargets(caster, skill, area, maxTargets);
				var hits = BonemancerSkillHelper.DealHits(caster, skill, targets, SkillModifier.MultiHit(HitCountPerTick));
				BonemancerSkillHelper.ApplyBasicDebuffs(caster, skill, hits.Select(hit => hit.Target), BuffId.Bonemancer_Rib_Debuff, BuffId.Bonemancer_Backbone_Debuff);
				if (tick + 1 < TickCount)
					await skill.Wait(TickInterval);
			}

			caster.RemoveBuff(BuffId.BoneStorm_Buff);
			Send.ZC_NORMAL.RemoveEffectByName(caster, "GroundAura_BoneStorm_White_01", true);
			Send.ZC_NORMAL.RemoveEffectByName(caster, "BodyAura_EmitDarkEnergy_Gray_01", true);
		}
	}
}
