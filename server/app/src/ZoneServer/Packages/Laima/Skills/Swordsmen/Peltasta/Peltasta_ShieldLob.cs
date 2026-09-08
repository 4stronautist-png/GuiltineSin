using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Abilities.Handlers.Swordsmen.Peltasta;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Peltasta
{
	/// <summary>
	/// Handler for the Peltasta skill Shield Lob.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Peltasta_ShieldLob)]
	public class Peltasta_ShieldLobOverride : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill, damaging targets.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.TurnTowards(farPos);
			caster.SetAttackState(true);

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			var pad = new Pad(PadName.Peltasta_ShieldLob, caster, skill, new Circle(caster.Position, 40));
			pad.Position = caster.Position.GetRelative(caster.Direction, 25);
			pad.Trigger.Subscribe(TriggerType.Enter, this.OnShieldCollision);

			caster.Map.AddPad(pad);
		}

		/// <summary>
		/// Called when an actor enters the shield's area.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnShieldCollision(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var target = args.Initiator;

			if (pad.Trigger.AtCapacity)
				return;

			if (!creator.CanDamage(target))
				return;

			pad.Trigger.ActivateCount++;
			this.Attack(pad.Skill, creator, target);
		}

		/// <summary>
		/// Attacks the target one time.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="target"></param>
		private void Attack(Skill skill, ICombatEntity caster, ICombatEntity target)
		{
			var aniTime = TimeSpan.FromMilliseconds(100);
			var skillHitDelay = TimeSpan.Zero;

			var modifier = SkillModifier.MultiHit(4);
			modifier.BonusPAtk = Peltasta38.GetBonusPAtk(caster);

			// Increase damage by 10% if target is under the effect of
			// Swashbuckling from the caster
			if (target.TryGetBuff(BuffId.SwashBuckling_Debuff, out var swashBuckingDebuff))
			{
				if (swashBuckingDebuff.Caster == caster)
					modifier.DamageMultiplier += 0.10f;
			}

			var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
			target.TakeDamage(skillHitResult.Damage, caster);

			var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, aniTime, skillHitDelay);
			skillHit.HitEffect = HitEffect.Impact;

			Send.ZC_SKILL_HIT_INFO(caster, skillHit);
		}
	}
}
