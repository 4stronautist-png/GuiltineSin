using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the JincanGu Debuff (GoldenFrog), which ticks poison damage every second.
	/// Deals double damage on the first 5 ticks.
	/// </summary>
	/// <remarks>
	/// NumArg1: None
	/// NumArg2: Snapshotted damage per tick (calculated on buff application)
	/// </remarks>
	[Package("laima")]
	[BuffHandler(BuffId.JincanGu_Abil_Debuff)]
	public class JincanGu_Abil_DebuffOverride : DamageOverTimeBuffHandler
	{
		private const string AdditionalHitsDoneVar = "AdditionalHitsDone";
		private const int AdditionalHitsMaxCount = 5;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			base.OnActivate(buff, activationType);
			buff.SetUpdateTime(3000);
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.Target.IsDead)
				return;

			// Apply first hit using base implementation (snapshotted damage)
			base.WhileActive(buff);

			// Apply second hit for the first 5 ticks
			var additionalHitsDone = buff.Vars.GetInt(AdditionalHitsDoneVar);
			if (additionalHitsDone < AdditionalHitsMaxCount)
			{
				var attacker = buff.Caster;
				var target = buff.Target;

				// Get the snapshotted damage
				target.TakeSimpleHit(buff.NumArg2, attacker, this.GetSkillId(buff), this.GetHitType(buff));

				buff.Vars.SetInt(AdditionalHitsDoneVar, additionalHitsDone + 1);
			}
		}

		protected override HitType GetHitType(Buff buff)
		{
			return HitType.Poison;
		}
	}
}
