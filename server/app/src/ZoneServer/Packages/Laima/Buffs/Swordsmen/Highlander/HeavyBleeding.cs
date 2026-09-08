using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Buff handler for HeavyBleeding, which deals bleed damage in regular intervals.
	/// </summary>
	/// <remarks>
	/// NumArg2: Snapshotted damage per tick (pre-calculated by skill handler)
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.HeavyBleeding)]
	public class HeavyBleedingOverride : DamageOverTimeBuffHandler
	{
		protected override HitType GetHitType(Buff buff)
		{
			return HitType.Bleeding;
		}
	}
}
