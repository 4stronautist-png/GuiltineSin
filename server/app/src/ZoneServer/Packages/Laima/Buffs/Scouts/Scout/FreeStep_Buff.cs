using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Scout
{
	/// <summary>
	/// Handle for the Free Step Buff, which increases the target's
	/// dodge rate.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.FreeStep_Buff)]
	public class FreeStep_BuffOverride : BuffHandler
	{
		private const float DodgeRateBaseBonus = 0.05f;
		private const float DodgeRateBonusPerLevel = 0.03f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var bonus = this.GetEvasionBonus(buff);

			AddPropertyModifier(buff, buff.Target, PropertyName.DR_RATE_BM, bonus);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.DR_RATE_BM);
		}

		private float GetEvasionBonus(Buff buff)
		{
			var skillLevel = buff.NumArg1;
			var bonus = DodgeRateBaseBonus + (skillLevel * DodgeRateBonusPerLevel);

			return bonus;
		}
	}
}
