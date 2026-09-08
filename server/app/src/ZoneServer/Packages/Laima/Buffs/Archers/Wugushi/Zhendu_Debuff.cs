using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Reduces Poison property resistance for Zhendu's attribute.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Zhendu_Debuff)]
	public class Zhendu_DebuffOverride : BuffHandler
	{
		private const float ResistanceReductionPerLevel = 0.10f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var abilityLevel = buff.NumArg1;
			var reductionRate = abilityLevel * ResistanceReductionPerLevel;
			var reduction = buff.Target.Properties.GetFloat(PropertyName.ResPoison) * reductionRate;
			AddPropertyModifier(buff, buff.Target, PropertyName.ResPoison_BM, -reduction);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.ResPoison_BM);
		}
	}
}
