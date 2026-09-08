using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Spurt, Move faster for 15 seconds..
	/// </summary>
	[BuffHandler(BuffId.Scud)]
	public class Scud : BuffHandler
	{
		private const int MovementBonus = 3;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.Properties.Modify(PropertyName.MSPD_BM, MovementBonus);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.Properties.Modify(PropertyName.MSPD_BM, -MovementBonus);
		}
	}
}
