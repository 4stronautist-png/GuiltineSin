using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Bulletmarker
{
	[Package("laima")]
	[SkillHandler(SkillId.Bulletmarker_TracerBullet)]
	public class Bulletmarker_TracerBullet : ISelfSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!BulletmarkerSkillHelper.TryStartSelf(skill, caster, originPos, dir))
				return;

			BulletmarkerSkillHelper.PlaySelfSkill(caster, skill);
			caster.StartBuff(BuffId.TracerBullet_Buff, skill.Level, 0, BulletmarkerSkillHelper.GetDefaultBuffDuration(), caster, skill.Id);

			if (caster is Character character)
				Send.ZC_OBJECT_PROPERTY(character);
		}
	}
}
