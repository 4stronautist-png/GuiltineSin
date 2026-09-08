using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Rage_stout buff, which increases Magic Attack Rate.
	/// </summary>
	[BuffHandler(BuffId.Rage_stout)]
	public class Rage_stout : BuffHandler
	{
		private const float MagicAttackRateBonus = 0.5f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.MATK_RATE_BM, MagicAttackRateBonus);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MATK_RATE_BM);
		}
	}
}
