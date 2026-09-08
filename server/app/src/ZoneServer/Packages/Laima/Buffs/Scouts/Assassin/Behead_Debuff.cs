using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Assassin
{
	/// <summary>
	/// Buff handler for Behead: Bleeding, which deals bleed damage in regular intervals.
	/// </summary>
	/// <remarks>
	/// NumArg2: Snapshotted damage per tick (pre-calculated by skill handler)
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.Behead_Debuff)]
	public class Behead_DebuffOverride : DamageOverTimeBuffHandler
	{
		protected override SkillId GetSkillId(Buff buff)
		{
			return SkillId.Assassin_Behead_DOT;
		}

		protected override HitType GetHitType(Buff buff)
		{
			return HitType.Bleeding;
		}
	}
}
