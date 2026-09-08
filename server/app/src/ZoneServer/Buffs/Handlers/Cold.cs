using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Chilly, Decreased movement speed..
	/// </summary>
	[BuffHandler(BuffId.Cold)]
	public class Cold : BuffHandler
	{
		private const int MovementPenalty = -30;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.SPD_BM, MovementPenalty);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.SPD_BM);
		}
	}
}
