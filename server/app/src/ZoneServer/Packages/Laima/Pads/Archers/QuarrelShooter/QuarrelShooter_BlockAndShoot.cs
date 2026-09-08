using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Zone.Skills;

namespace GuiltineSin.Zone.Pads.Handlers
{
	[Package("laima")]
	[PadHandler(PadName.QuarrelShooter_BlockAndShoot)]
	public class QuarrelShooter_BlockAndShootOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, IUpdatePadHandler
	{
		/// <summary>
		/// Initializes the pad when created.
		/// </summary>
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = args.Skill;

			Send.ZC_NORMAL.PadUpdate(creator, pad, pad.Name, -2.356194f, 0, 30, true);
			pad.SetRange(30f);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(1000);
			pad.Trigger.Area = new Circle(pad.Position, 30f);
			var value = (int)(skill.Data.SplashRate + creator.Properties.GetFloat(PropertyName.SR));
			pad.Trigger.MaxUseCount = value;
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		/// <summary>
		/// Handles an entity entering the pad area.
		/// </summary>
		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			this.Attack(pad, skill, creator, initiator);
		}

		/// <summary>
		/// Handles periodic updates of the pad.
		/// </summary>
		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;
			var sr = creator.Properties.GetFloat(PropertyName.SR);
		}

		private void Attack(Pad pad, Skill skill, ICombatEntity creator, ICombatEntity target)
		{
			if (pad.Trigger.AtCapacity)
				return;

			if (!creator.CanDamage(target))
				return;

			if (target.IsDead)
				return;

			var skillHitResult = SCR_SkillHit(creator, target, skill);
			target.TakeDamage(skillHitResult.Damage, creator);

			var hit = new HitInfo(creator, target, skill, skillHitResult);
			Send.ZC_HIT_INFO(creator, target, hit);

			pad.Trigger.IncreaseUseCount();
		}
	}
}
