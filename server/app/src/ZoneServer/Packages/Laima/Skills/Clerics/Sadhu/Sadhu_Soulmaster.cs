using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sadhu
{
	/// <summary>
	/// Handler for the Cleric skill Spirit Expert (Soul Master).
	/// This skill is no longer in the skill tree (replaced by Out of Body).
	/// Kept as a no-op to prevent errors if triggered.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sadhu_Soulmaster)]
	public class Sadhu_SoulmasterOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
		}
	}
}
