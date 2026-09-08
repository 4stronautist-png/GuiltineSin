using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[Package("laima")]
	[PadHandler(PadName.Pyromancer_HellBreath)]
	public class Pyromancer_HellBreathOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler
	{
		private const int KnockbackDistance = 30;
		private const int KnockbackVelocityPerAbilityLevel = 5;

		/// <summary>
		/// Initializes the Hell Breath pad
		/// </summary>
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(30);
			pad.Trigger.MaxConcurrentUseCount = 2;
		}

		/// <summary>
		/// Cleans up the Hell Breath pad
		/// </summary>
		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		/// <summary>
		/// Applies damage when the pad enters an enemy
		/// </summary>
		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var target = args.Initiator;

			if (!creator.IsEnemy(target))
				return;

			if (pad.Trigger.AtCapacity)
				return;

			pad.Trigger.ActivateCount++;
			var skill = pad.Skill;
			var skillHitResult = SCR_SkillHit(creator, target, skill);
			target.TakeDamage(skillHitResult.Damage, creator);

			var hitInfo = new HitInfo(creator, target, skill, skillHitResult.Damage, HitResultType.Hit);
			Send.ZC_HIT_INFO(creator, target, hitInfo);

			// Apply fire effect
			target.PlayEffect("F_hit_fire", 1f);

			if (creator.TryGetActiveAbility(AbilityId.Pyromancer4, out var ability))
				this.ApplyKnockback(pad, creator, target, ability.Level);
		}

		private void ApplyKnockback(Pad pad, ICombatEntity creator, ICombatEntity initiator, int abilityLevel)
		{
			if (!initiator.IsKnockdownable())
				return;

			var skill = pad.Skill;
			if (skill == null)
				return;

			var knockbackDirection = pad.Position.GetDirection(initiator.Position);
			var knockbackVelocity = KnockbackVelocityPerAbilityLevel * abilityLevel;

			var skillHitResult = new SkillHitResult { Damage = 0, Result = HitResultType.Hit };
			var skillHit = new SkillHitInfo(creator, initiator, skill, skillHitResult);

			skillHit.KnockBackInfo = new KnockBackInfo(pad.Position, initiator, KnockBackType.KnockBack, knockbackVelocity, 10);
			skillHit.HitInfo.KnockBackType = KnockBackType.KnockBack;

			initiator.ApplyKnockback(creator, skill, skillHit);

			Send.ZC_SKILL_HIT_INFO(creator, skillHit);
		}
	}
}
