using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Skills.Handlers.Base;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the passive Corsair skill Brutality.
	/// Buff application is handled by JollyRoger_Enemy_Debuff's combat
	/// modifier, which fires for all attackers hitting flagged enemies.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Corsair_Brutality)]
	public class Corsair_BrutalityOverride : ISkillHandler
	{
	}
}
