using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Divine Stigma, Receive continuous damage, Ignores some of your Defense when hit.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.DivineStigma_Damage_Debuff)]
	public class DivineStigma_Damage_DebuffOverride : BuffHandler
	{

	}
}
