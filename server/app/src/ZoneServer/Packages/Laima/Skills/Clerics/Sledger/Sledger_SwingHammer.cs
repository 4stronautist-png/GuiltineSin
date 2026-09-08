using System;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sledger
{
	/// <summary>
	/// Handles Swing Hammer.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sledger_SwingHammer_Cleric)]
	public class Sledger_SwingHammerOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!SledgerSkillHelper.TryStart(skill, caster, originPos, farPos, visualPos: caster.Position))
				return;

			if (caster.TryGetActiveAbilityLevel(AbilityId.Sledger14, out var warmupLevel))
				caster.StartBuff(BuffId.Sledger_Preheat_Buff, skill.Level, warmupLevel, SledgerSkillHelper.WarmupDuration, caster, skill.Id);

			SledgerSkillHelper.RunCancellable(skill, () => SledgerSkillHelper.AttackAreaOverTime(
				skill,
				caster,
				() => SledgerSkillHelper.CreateCircle(skill, caster, caster.Position, radius: 36),
				skill.Data.MultiHitCount,
				SledgerSkillHelper.GetSkillActionDuration(skill, 1500),
				appliesBigBangReduction: true,
				bigBangReductionSeconds: 1
			));
		}
	}
}
