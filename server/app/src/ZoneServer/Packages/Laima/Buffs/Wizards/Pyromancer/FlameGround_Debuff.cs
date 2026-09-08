using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Shared.Data.Database;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizard
{
	/// <summary>
	/// Handler for the Flame Ground debuff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.FlameGround_Debuff)]
	public class FlameGround_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.Fire_Def_BM, -buff.NumArg2 * 50);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.Fire_Def_BM);
		}
	}
}
