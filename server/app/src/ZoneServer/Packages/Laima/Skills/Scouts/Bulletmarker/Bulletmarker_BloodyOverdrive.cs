using System;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_BloodyOverdrive)]
	public class Bulletmarker_BloodyOverdrive : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BulletmarkerSkillHelper.TryStart(skill, caster, originPos, farPos))
				return;

			var broadside = caster.IsAbilityActive(AbilityId.Bulletmarker25);
			var hitCount = Math.Max(1, skill.Data.MultiHitCount);
			if (BulletmarkerSkillHelper.TryConsumeOutrage(caster))
				hitCount = Math.Max(1, (int)Math.Round(hitCount * 0.75f));

			var modifier = BulletmarkerSkillHelper.CreateModifier(skill, 1);
			var animationTime = TimeSpan.FromMilliseconds(broadside ? 110 : 420);
			var immovableTime = TimeSpan.FromMilliseconds(skill.Data.ShootTime.TotalMilliseconds > 0 ? skill.Data.ShootTime.TotalMilliseconds : 1700);

			caster.StartBuff(BuffId.BloodyOverdrive_Immovable_Buff, skill.Level, 0, immovableTime, caster, skill.Id);

			if (caster.IsAbilityActive(AbilityId.Bulletmarker12))
				caster.StartBuff(BuffId.Invincible, skill.Level, 0, TimeSpan.FromSeconds(1), caster, skill.Id);

			var ricochetLevel = caster.GetAbilityLevel(AbilityId.Bulletmarker8);
			var ricochetChance = Math.Min(100, ricochetLevel * 5);

			skill.Run(BulletmarkerSkillHelper.AttackBloodyOverdrive(
				skill,
				caster,
				farPos,
				modifier,
				hitCount,
				ricochetChance,
				animationTime: animationTime));
		}
	}
}
