using System;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_RestInPeace)]
	public class Bulletmarker_RestInPeace : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BulletmarkerSkillHelper.TryStart(skill, caster, originPos, farPos))
				return;

			var modifier = BulletmarkerSkillHelper.CreateModifier(skill);
			if (BulletmarkerSkillHelper.TryConsumeOutrage(caster))
				modifier.FinalDamageMultiplier *= 1.55f;

			if (caster.IsAbilityActive(AbilityId.Bulletmarker13))
				modifier.HitCount += 1;

			skill.Run(BulletmarkerSkillHelper.AttackArea(skill, caster, originPos, farPos, modifier, length: 130, width: 80, angle: 35, splashType: SplashType.Square));
		}
	}
}
