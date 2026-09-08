using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Falconer
{
	/// <summary>
	/// Handle for the Aiming buff applied to enemies.
	/// Increases the target's effective hit radius for AoE attacks
	/// without affecting pathfinding.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Aiming_Buff)]
	public class Falconer_Aiming_BuffOverride : BuffHandler
	{
		private const float AimingHitRadiusBonus = 10.00f;
		private const float AimingHitRadiusBonusPerLevel = 2.0f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var level = buff.NumArg1;
			buff.Target.HitRadiusBonus = AimingHitRadiusBonus + level * AimingHitRadiusBonusPerLevel;

			Send.ZC_NORMAL.EnableHitRadiusPreview(buff.Target, true, buff.Caster);
			Send.ZC_ALTER_HIT_RADIUS(buff.Caster, buff.Target, AimingHitRadiusBonus, 0f);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.HitRadiusBonus = 0f;

			Send.ZC_NORMAL.EnableHitRadiusPreview(buff.Target, false, buff.Caster);
			Send.ZC_ALTER_HIT_RADIUS(buff.Caster, buff.Target, 0f, 0f);
		}
	}
}
