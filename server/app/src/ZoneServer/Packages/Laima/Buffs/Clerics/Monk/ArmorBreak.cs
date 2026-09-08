using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handler for the ArmorBreak debuff applied by Hand Knife.
	/// Reduces physical defense by 2% per ability level of Monk5.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.ArmorBreak)]
	public class ArmorBreakOverride : BuffHandler
	{
		private const float DefRatePerLevel = 0.02f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var def = target.Properties.GetFloat(PropertyName.DEF);
			var penalty = def * DefRatePerLevel * buff.NumArg1;

			AddPropertyModifier(buff, target, PropertyName.DEF_BM, -penalty);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.DEF_BM);
		}
	}
}
