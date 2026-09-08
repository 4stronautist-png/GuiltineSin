using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.OutLaw
{
	/// <summary>
	/// Handler for Break Brick Debuff, which reduces Crit Chance
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: None
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.BreakBrick_Debuff)]
	internal class BreakBrick_DebuffOverride : BuffHandler
	{
		private const float CRTPenaltyPerLevel = 1f;

		/// <summary>
		/// Starts buff, reducing Crit rate
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var reduceCrt = buff.Target.Properties.GetFloat(PropertyName.CRTHR) * buff.NumArg1 * CRTPenaltyPerLevel;

			AddPropertyModifier(buff, buff.Target, PropertyName.CRTHR_BM, -reduceCrt);
		}

		/// <summary>
		/// Ends the buff, resetting Crit rate.
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.CRTHR_BM);
		}
	}
}
