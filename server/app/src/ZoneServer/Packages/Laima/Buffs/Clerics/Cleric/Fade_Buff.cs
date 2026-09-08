using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Fade Buff, which lowers the target's threat levels
	/// and increases magic defense.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Fade_Buff)]
	public class Fade_BuffOverride : BuffHandler
	{
		private const float BaseMagicDefenseRate = 0.10f;
		private const float MagicDefenseRatePerLevel = 0.04f;

		/// <summary>
		/// Starts buff
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var skillLevel = buff.NumArg1;
			var rate = BaseMagicDefenseRate + MagicDefenseRatePerLevel * skillLevel;

			AddPropertyModifier(buff, buff.Target, PropertyName.MDEF_RATE_BM, rate);
		}

		/// <summary>
		/// Ends the buff
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MDEF_RATE_BM);
		}
	}
}
