using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handle for the Barrier: Might, Physical Damage Increase.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Barrier_MeleeAttack_Buff)]
	public class Barrier_MeleeAttack_BuffOverride : BuffHandler
	{
		private const float Rate = 0.10f;
		private const string ModPropertyName = PropertyName.PATK_BM;

		/// <summary>
		/// Starts buff
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var modValue = Rate * buff.NumArg1;

			AddPropertyModifier(buff, target, ModPropertyName, modValue);
		}

		/// <summary>
		/// Ends the buff
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, ModPropertyName);
		}
	}
}
