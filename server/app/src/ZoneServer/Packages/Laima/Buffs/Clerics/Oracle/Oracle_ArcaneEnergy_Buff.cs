using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Oracle
{
	/// <summary>
	/// Handle for the Arcane Energy buff, which increases MSP and
	/// max stamina while active.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ArcaneEnergy_Buff)]
	public class Oracle_ArcaneEnergy_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var buffArg1 = buff.NumArg1;

			var baseMsp = target.Properties.GetFloat(PropertyName.MSP) - target.Properties.GetFloat(PropertyName.MSP_BM);
			var addSp = (float)Math.Floor(baseMsp * 0.03f * buffArg1);
			var addSta = 5f + buffArg1 * 4f;

			AddPropertyModifier(buff, target, PropertyName.MSP_BM, addSp);
			AddPropertyModifier(buff, target, PropertyName.MaxSta_BM, addSta);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSP_BM);
			RemovePropertyModifier(buff, buff.Target, PropertyName.MaxSta_BM);
		}
	}
}
