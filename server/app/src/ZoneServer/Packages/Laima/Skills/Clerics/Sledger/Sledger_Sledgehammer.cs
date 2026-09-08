using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sledger
{
	/// <summary>
	/// Passive marker for Sledgehammer. Its defense ignore is applied by
	/// SledgerSkillHelper when Sledger attacks hit.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sledger_Sledgehammer_Cleric)]
	public class Sledger_SledgehammerOverride : ISelfSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			caster.StartBuff(BuffId.Sledgehammer_Buff, skill.Level, 0, System.TimeSpan.Zero, caster, skill.Id);
		}
	}
}
