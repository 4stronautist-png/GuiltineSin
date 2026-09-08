using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Clerics.Dievdirbys
{
	/// <summary>
	/// Handler override for Ausirine_Buff, which provides resistance to debuffs
	/// while standing near the Ausrine statue.
	/// Resistance: 30% base + 3% per skill level
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Ausirine_Buff)]
	public class Ausirine_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			// NumArg1 contains the CarveAusrine skill level from the pad handler
			// This is used in BuffComponent.TryResistDebuff to calculate resistance chance
			// Resistance = 30 + (3 * skill level)
		}

		public override void OnEnd(Buff buff)
		{
			// No cleanup needed - resistance is checked directly from buff presence
		}
	}
}
