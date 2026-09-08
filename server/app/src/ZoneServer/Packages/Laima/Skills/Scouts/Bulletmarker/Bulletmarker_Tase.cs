using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_Tase)]
	public class Bulletmarker_Tase : IForceSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!BulletmarkerSkillHelper.TryStart(skill, caster, originPos, farPos))
				return;

			var magazineBonus = caster.TryGetActiveAbilityLevel(AbilityId.Bulletmarker7, out var extraMagazineLevel) ? extraMagazineLevel : 0;
			var modifier = BulletmarkerSkillHelper.CreateModifier(skill, skill.Data.MultiHitCount + magazineBonus);

			skill.Run(BulletmarkerSkillHelper.AttackTarget(skill, caster, target, modifier, (hitTarget, _) =>
				hitTarget.StartBuff(BuffId.Tase_Debuff, skill.Level, 10 + magazineBonus, TimeSpan.FromSeconds(10), caster, skill.Id)));
		}
	}
}
