using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Frimveil_Wizard)]
	public class AetherBlader_FrimveilWizard : AetherBlader_Frimveil
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Frimveil_Cleric)]
	public class AetherBlader_FrimveilCleric : AetherBlader_Frimveil
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Frimveil_Scout)]
	public class AetherBlader_FrimveilScout : AetherBlader_Frimveil
	{
	}

	public class AetherBlader_Frimveil : IMeleeGroundSkillHandler, IGroundSkillHandler, IDynamicCasted
	{
		private const float RushDistance = 100f;
		private const int IceBladeItemId = 121135;
		private const int IceBladeEffectStringId = 1827;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Handle(skill, caster, originPos, farPos, targets?.FirstOrDefault());

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var frozenStance = caster.IsAbilityActive(AbilityId.AetherBlader7);
			var spMultiplier = frozenStance ? 1.3f : 1f;
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
				return;

			skill.IncreaseOverheat();

			var direction = AetherBladerSkillHelper.GetCasterForwardDirection(caster);
			var startPos = caster.Position;
			var maxRange = skill.Properties.GetFloat(PropertyName.MaxR);
			if (maxRange <= 0)
				maxRange = RushDistance;
			var endPos = frozenStance ? startPos : caster.Map.Ground.GetLastValidPosition(startPos, startPos.GetRelative(direction, maxRange));
			var area = new Square(startPos, direction, maxRange, skill.Properties.GetFloat(PropertyName.SplRange));

			this.ShowBlade(skill, caster);
			this.PrepareFrimveilSkill(skill, caster, startPos, endPos, direction, frozenStance);
			skill.Run(this.HideBlade(skill, caster));

			if (!frozenStance)
				caster.Position = endPos;

			var coldEcho = caster.IsAbilityActive(AbilityId.AetherBlader8);
			var targetsInArea = AetherBladerSkillHelper.GetTargets(caster, skill, area);
			AetherBladerSkillHelper.DealHits(caster, skill, targetsInArea, t =>
			{
				var bonus = AetherBladerSkillHelper.GetTideCallFinalDamageBonus(caster);
				if (t.IsBuffActive(BuffId.Wet_Debuff))
					bonus += 0.30f;
				if (coldEcho)
					bonus -= 0.25f;
				return AetherBladerSkillHelper.BuildModifier(skill, t, bonus);
			});

			if (coldEcho)
			{
				var coldEchoArea = new Square(startPos, direction, maxRange + AetherBladerSkillHelper.PuddleRadius, skill.Properties.GetFloat(PropertyName.SplRange) + AetherBladerSkillHelper.PuddleRadius);
				var puddles = AetherBladerSkillHelper.GetPuddles(caster)
					.Where(pad => AetherBladerSkillHelper.IsPuddleReachedByArea(coldEchoArea, pad))
					.ToList();

				foreach (var puddle in puddles)
				{
					var pillarPos = puddle.Position;
					AetherBladerSkillHelper.DestroyPuddle(puddle);
					AetherBladerSkillHelper.CreateFrostPillar(caster, skill, pillarPos);
				}
			}
		}

		private void ShowBlade(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.AetherBlader_IceBlade_Buff, skill.Level, 0, System.TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true, IceBladeItemId, IceBladeEffectStringId);
		}

		private async System.Threading.Tasks.Task HideBlade(Skill skill, ICombatEntity caster)
		{
			await skill.Wait(System.TimeSpan.FromMilliseconds(860));
			caster.RemoveBuff(BuffId.AetherBlader_IceBlade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false, IceBladeItemId, IceBladeEffectStringId);
		}

		private void PrepareFrimveilSkill(Skill skill, ICombatEntity caster, Position startPos, Position endPos, Direction direction, bool frozenStance)
		{
			caster.SetAttackState(true);
			Send.ZC_SKILL_READY(caster, skill, startPos, startPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, startPos, direction, Position.Zero);
			if (!frozenStance)
				Send.ZC_NORMAL.SkillMoveJump(caster, endPos);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, startPos, ForceId.GetNew(), null);
		}
	}
}
