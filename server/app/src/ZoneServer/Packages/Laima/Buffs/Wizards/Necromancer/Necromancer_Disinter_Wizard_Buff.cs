using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Necromancer
{
	/// <summary>
	/// Handle for the Sacrifice: Skeleton Mage, Immune to Debuff.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Disinter_Wizard_Buff)]
	public class Necromancer_Disinter_Wizard_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
