using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Swordsmen.Hoplite
{
	/// <summary>
	/// Handler override for the Stabbing Debuff.
	/// This debuff stacks indefinitely, increasing damage taken from Stabbing by 2% per stack.
	/// The damage calculation is handled in the skill handler.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Stabbing_Debuff)]
	public class Stabbing_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			// No property modifications needed - damage increase is calculated in skill handler
			// The buff simply tracks stacks via OverbuffCounter
		}

		public override void OnEnd(Buff buff)
		{
			// No cleanup needed
		}
	}
}
