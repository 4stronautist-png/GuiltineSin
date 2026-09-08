using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for drop_inceaseMoney, which stores values for loot calculation.
	/// </summary>
	[BuffHandler(BuffId.drop_inceaseMoney)]
	public class drop_inceaseMoney : BuffHandler
	{
		private const string VarMoneyCount = "GuiltineSin.Drop.MoneyCount";
		private const string VarMoneyRatio = "GuiltineSin.Drop.MoneyRatio";

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			// This buff's purpose is to hold values for the drop system to read.
			// It stores its arguments into its own variables.
			buff.Vars.SetFloat(VarMoneyCount, buff.NumArg1);
			buff.Vars.SetFloat(VarMoneyRatio, buff.NumArg2);
		}
	}
}
