using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Cataphract
{
	/// <summary>
	/// Handler for Warrior_EnableMovingShot_Buff.
	/// Enables attacking while moving.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Warrior_EnableMovingShot_Buff)]
	public class Warrior_EnableMovingShot_Buff_Handler : BuffHandler
	{
		private const float Rate = 1.5f;
		private const string ModPropertyName = PropertyName.MovingShot_BM;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var modValue = buff.NumArg1 * Rate;

			AddPropertyModifier(buff, buff.Target, ModPropertyName, modValue);

			buff.Target.Properties.Invalidate(PropertyName.MSPD);
			Send.ZC_MOVE_SPEED(buff.Target);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, ModPropertyName);

			buff.Target.Properties.Invalidate(PropertyName.MSPD);
			Send.ZC_MOVE_SPEED(buff.Target);
		}
	}
}
