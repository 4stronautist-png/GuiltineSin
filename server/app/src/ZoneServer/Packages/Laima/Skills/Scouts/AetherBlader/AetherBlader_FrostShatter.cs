using System.Collections.Generic;
using System.Linq;
using Melia.Shared.Data.Database;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.SplashAreas;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_FrostShatter_Wizard)]
	public class AetherBlader_FrostShatterWizard : AetherBlader_FrostShatter
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_FrostShatter_Cleric)]
	public class AetherBlader_FrostShatterCleric : AetherBlader_FrostShatter
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_FrostShatter_Scout)]
	public class AetherBlader_FrostShatterScout : AetherBlader_FrostShatter
	{
	}

	public class AetherBlader_FrostShatter : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const int IceBladeDurationMs = 1300;
		private const int IceBladeItemId = 121135;
		private const int IceBladeEffectStringId = 1827;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var spMultiplier = caster.IsAbilityActive(AbilityId.AetherBlader9) ? 2f : 1f;
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
				return;

			skill.IncreaseOverheat();

			var direction = AetherBladerSkillHelper.GetCasterForwardDirection(caster);
			var targetPos = AetherBladerSkillHelper.GetForwardPosition(caster, skill.Properties.GetFloat(PropertyName.MaxR));
			this.ShowBlade(skill, caster);
			AetherBladerSkillHelper.PrepareGroundSkill(skill, caster, caster.Position, targetPos, direction);
			skill.RunFree(this.HideBlade(caster));

			if (caster.IsAbilityActive(AbilityId.AetherBlader9))
				this.CastShatterline(skill, caster, direction);
			else
				this.CastFromPuddles(skill, caster, direction);
		}

		private void CastFromPuddles(Skill skill, ICombatEntity caster, Direction direction)
		{
			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR) + AetherBladerSkillHelper.PuddleRadius;
			var width = skill.Properties.GetFloat(PropertyName.SplRange) + AetherBladerSkillHelper.PuddleRadius;
			var area = new Square(caster.Position, direction, maxRange, width);
			var puddles = AetherBladerSkillHelper.GetPuddles(caster).Where(pad => AetherBladerSkillHelper.IsPuddleReachedByArea(area, pad)).ToList();
			foreach (var puddle in puddles)
				this.CreatePillarAndHit(skill, caster, puddle);
		}

		private void CastShatterline(Skill skill, ICombatEntity caster, Direction direction)
		{
			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR) + AetherBladerSkillHelper.PuddleRadius;
			var area = new Square(caster.Position, direction, maxRange, 20f + AetherBladerSkillHelper.PuddleRadius);
			var puddles = AetherBladerSkillHelper.GetPuddles(caster).Where(pad => AetherBladerSkillHelper.IsPuddleReachedByArea(area, pad)).ToList();
			foreach (var puddle in puddles)
				this.CreatePillarAndHit(skill, caster, puddle);
		}

		private void ShowBlade(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.AetherBlader_IceBlade_Buff, skill.Level, 0, System.TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true, IceBladeItemId, IceBladeEffectStringId);
		}

		private async System.Threading.Tasks.Task HideBlade(ICombatEntity caster)
		{
			await System.Threading.Tasks.Task.Delay(IceBladeDurationMs);
			caster.RemoveBuff(BuffId.AetherBlader_IceBlade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false, IceBladeItemId, IceBladeEffectStringId);
		}

		private void CreatePillarAndHit(Skill skill, ICombatEntity caster, Melia.Zone.World.Actors.Pads.Pad puddle)
		{
			var position = puddle.Position;
			AetherBladerSkillHelper.DestroyPuddle(puddle);
			this.CreatePillarAndHit(skill, caster, position);
		}

		private void CreatePillarAndHit(Skill skill, ICombatEntity caster, Position position)
		{
			AetherBladerSkillHelper.CreateFrostPillar(caster, skill, position);

			var area = new Circle(position, AetherBladerSkillHelper.FrostPillarRadius);
			var targets = AetherBladerSkillHelper.GetTargets(caster, skill, area);
			var hits = AetherBladerSkillHelper.DealHits(caster, skill, targets, t => AetherBladerSkillHelper.BuildModifier(skill, t, AetherBladerSkillHelper.GetTideCallFinalDamageBonus(caster)));

			foreach (var target in hits.Select(hit => hit.Target))
			{
				if (target.IsBuffActive(BuffId.Wet_Debuff))
					target.StartBuff(BuffId.FrostSatter_Debuff, skill.Level, 0, AetherBladerSkillHelper.FrozenDuration, caster, skill.Id);
			}
		}
	}
}
