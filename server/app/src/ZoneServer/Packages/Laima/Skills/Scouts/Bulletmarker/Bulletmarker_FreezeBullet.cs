using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_FreezeBullet)]
	public class Bulletmarker_FreezeBullet : ISelfSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (caster.IsBuffActive(BuffId.Outrage_Buff))
			{
				Send.ZC_SKILL_DISABLE(caster);
				return;
			}

			if (!BulletmarkerSkillHelper.TryStartSelf(skill, caster, originPos, dir))
				return;

			BulletmarkerSkillHelper.PlaySelfSkill(caster, skill);

			var duration = TimeSpan.FromSeconds(8);
			caster.StartBuff(BuffId.FreezeBullet_Buff, skill.Level, 0, duration, caster, skill.Id);

			if (caster.IsAbilityActive(AbilityId.Bulletmarker24))
				caster.StartBuff(BuffId.SilverBullet_Buff, skill.Level, 0, duration, caster, skill.Id);

			if (caster is Character character)
			{
				Send.ZC_OBJECT_PROPERTY(character);
				Send.ZC_MOVE_SPEED(character);
			}
		}
	}
}
