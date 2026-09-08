using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsman.Barbarian
{
	/// <summary>
	/// Handler for Warcry_Buff, which increases physical attack.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: Stacks
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.Warcry_Buff)]
	public class Warcry_BuffOverride : BuffHandler
	{
		private const float PAtkBonusPerStack = 0.025f;

		/// <summary>
		/// Starts buff, increasing PAtk
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var bonusPatk = buff.Target.Properties.GetFloat(PropertyName.PATK) * (buff.NumArg2 * PAtkBonusPerStack);

			AddPropertyModifier(buff, buff.Target, PropertyName.PATK_BM, bonusPatk);
		}

		/// <summary>
		/// Ends the buff, resetting PAtk.
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.PATK_BM);
		}
	}
}
