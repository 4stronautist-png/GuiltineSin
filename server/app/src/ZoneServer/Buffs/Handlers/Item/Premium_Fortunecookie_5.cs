using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	[BuffHandler(BuffId.Premium_Fortunecookie_5)]
	public class Premium_Fortunecookie_5 : Premium_Fortunecookie_Base
	{
		private const int MspdBonus = 5;
		private const int MhpBonus = 2000;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.Properties.Modify(PropertyName.MSPD_BM, MspdBonus);
			buff.Target.Properties.Modify(PropertyName.MHP_BM, MhpBonus);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.Properties.Modify(PropertyName.MSPD_BM, -MspdBonus);
			buff.Target.Properties.Modify(PropertyName.MHP_BM, -MhpBonus);
		}
	}
}
