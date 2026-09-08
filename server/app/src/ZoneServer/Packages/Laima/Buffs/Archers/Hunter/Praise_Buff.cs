using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handler for the Praise Buff, which increases movement speed.
	/// </summary>
	/// <remarks>
	/// NumArg1: The skill level.
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.Praise_Buff)]
	public class Praise_BuffOverride : BuffHandler
	{
		private const float BaseMspdBonus = 10f;
		private const float MspdBonusPerLevel = 2f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var skillLevel = buff.NumArg1;

			var mspdBonus = BaseMspdBonus + skillLevel * MspdBonusPerLevel;
			AddPropertyModifier(buff, target, PropertyName.MSPD_BM, mspdBonus);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			RemovePropertyModifier(buff, target, PropertyName.MSPD_BM);
		}
	}
}
