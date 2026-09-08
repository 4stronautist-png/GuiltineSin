using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Bokor
{
	/// <summary>
	/// Handler override for the Decomposition Debuff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Decomposition_Debuff)]
	public class Decomposition_DebuffOverride : BuffHandler
	{
	}
}
