using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Swordsmen.Highlander
{
	/// <summary>
	/// Handler for SkullSwing_Debuff, which reduces defense.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill Level
	/// NumArg2: Defense Reduction Multiplier
	/// </remarks>
	[BuffHandler(BuffId.ScullSwing_Debuff)]
	public class ScullSwing_Debuff : BuffHandler
	{
		/// <summary>
		/// Starts buff, reducing Def
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var reduceDef = target.Properties.GetFloat(PropertyName.DEF) * buff.NumArg2;

			AddPropertyModifier(buff, target, PropertyName.DEF_BM, -reduceDef);
		}

		/// <summary>
		/// Ends the buff, resetting Def
		/// </summary>
		/// <param name="buff"></param>
		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.DEF_BM);
		}
	}
}
