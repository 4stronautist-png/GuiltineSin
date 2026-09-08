using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Archers.PiedPiper;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Mon
{
	[SkillHandler(SkillId.Mon_piedpiper_mouse_Skill_1)]
	public class Mon_piedpiper_mouse_Skill_1 : ITargetSkillHandler
	{
		protected TimeSpan AniTime { get; } = TimeSpan.FromMilliseconds(450);
		public void Handle(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var originPos = caster.Position;
			var hitDelay = 250;
			var aniTime = 450;
			var leadPos = GetLeadPosition(target, hitDelay, caster);
			caster.TurnTowards(leadPos);
			var farPos = originPos.GetNearestPositionWithinDistance(leadPos, skill.Properties[PropertyName.MaxR]);
			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);

			skill.Run(this.HandleSkill(caster, target, skill, originPos, farPos, hitDelay, aniTime));
		}

		private async Task HandleSkill(ICombatEntity caster, ICombatEntity target, Skill skill, Position originPos, Position farPos, int hitDelay, int aniTime)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 25, width: 30, angle: 30f);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);
			var hits = new List<SkillHitInfo>();
			await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, hits);
			ApplyRatMark(caster, skill, target);
		}

		public static void ApplyRatMark(ICombatEntity caster, Skill skill, ICombatEntity target)
		{
			if (target == null || target.IsDead)
				return;

			var ownerHandle = caster is IMonster monster ? monster.OwnerHandle : 0;
			var owner = caster.Map?.TryGetCharacter(ownerHandle, out var character) == true
				? character
				: caster as Character;
			var rare = caster is Mob mob && mob.Id == PiedPiperSkillHelper.RareMouseId || skill.Id == SkillId.Mon_piedpiper_mouse_White_Skill_1;
			PiedPiperSkillHelper.ApplyBestFriendMark(owner, skill, target, rare);
		}
	}

	[SkillHandler(SkillId.Mon_piedpiper_mouse_White_Skill_1)]
	public class Mon_piedpiper_mouse_White_Skill_1 : ITargetSkillHandler
	{
		protected TimeSpan AniTime { get; } = TimeSpan.FromMilliseconds(450);
		public void Handle(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var originPos = caster.Position;
			var hitDelay = 250;
			var aniTime = 450;
			var leadPos = GetLeadPosition(target, hitDelay, caster);
			caster.TurnTowards(leadPos);
			var farPos = originPos.GetNearestPositionWithinDistance(leadPos, skill.Properties[PropertyName.MaxR]);
			var forceId = ForceId.GetNew();
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, forceId, null);

			skill.Run(this.HandleSkill(caster, target, skill, originPos, farPos, hitDelay, aniTime));
		}

		private async Task HandleSkill(ICombatEntity caster, ICombatEntity target, Skill skill, Position originPos, Position farPos, int hitDelay, int aniTime)
		{
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 25, width: 30, angle: 30f);
			var splashArea = skill.GetSplashArea(SplashType.Circle, splashParam);
			var hits = new List<SkillHitInfo>();
			await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, hits);
			Mon_piedpiper_mouse_Skill_1.ApplyRatMark(caster, skill, target);
		}
	}

}
