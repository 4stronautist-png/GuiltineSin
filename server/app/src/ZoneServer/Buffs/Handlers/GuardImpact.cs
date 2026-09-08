using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Defense Shock, Blocked by enemy becomes afflicted with [Shock]. 
	/// Increases the chance of a critical hit from enemy attacks by 100%..
	/// </summary>
	[BuffHandler(BuffId.GuardImpact)]
	public class GuardImpact : BuffHandler
	{
		private const int BonusCriticalDR = -10000;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			target.Properties.Modify(PropertyName.CRTDR_BM, BonusCriticalDR);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			target.Properties.Modify(PropertyName.CRTDR_BM, -BonusCriticalDR);
		}
	}
}
