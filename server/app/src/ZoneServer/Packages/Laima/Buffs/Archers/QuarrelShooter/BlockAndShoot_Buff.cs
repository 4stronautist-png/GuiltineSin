using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handler for BlockAndShoot_Buff, which increases block
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.BlockAndShoot_Buff)]
	public class BlockAndShootOverride : BuffHandler
	{
		/// <summary>
		/// Applies the buff effects when activated.
		/// </summary>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var blockBonus = buff.NumArg2;

			AddPropertyModifier(buff, target, PropertyName.BLK_BM, blockBonus);
		}

		/// <summary>
		/// Removes the buff effects when it ends.
		/// </summary>
		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			RemovePropertyModifier(buff, target, PropertyName.BLK_BM);
		}
	}
}
