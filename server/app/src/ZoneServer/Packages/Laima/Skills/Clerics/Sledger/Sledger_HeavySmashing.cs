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
	/// Handles Heavy Smashing.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sledger_HeavySmashing_Cleric)]
	public class Sledger_HeavySmashingOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!SledgerSkillHelper.TryStart(skill, caster, originPos, farPos, visualPos: caster.Position))
				return;

			var duration = SledgerSkillHelper.GetSkillActionDuration(skill, 1500);
			SledgerSkillHelper.RunCancellable(skill, () => SledgerSkillHelper.AttackAreaOverTime(
				skill,
				caster,
				() => SledgerSkillHelper.CreateImpactSquare(skill, caster, SledgerSkillHelper.GetImpactPosition(caster, farPos, 12), length: 24, width: 24),
				skill.Data.MultiHitCount,
				duration,
				appliesBigBangReduction: true,
				bigBangReductionSeconds: 1
			));
		}
	}
}
