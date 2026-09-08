using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Pads.Handlers
{
	/// <summary>
	/// Pad handler for Falconer's Circling skill.
	/// Follows the hawk companion. Applies UC_Detected_Debuff to enemies.
	/// </summary>
	[Package("laima")]
	[PadHandler(PadName.Falconer_Circling)]
	public class Falconer_CirclingOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(80f);
			pad.SetUpdateInterval(1000);

			// Follow the hawk, not the caster
			if (creator is Character character)
			{
				var hawk = character.Companions.ActiveBirdCompanion;
				if (hawk != null)
					pad.FollowsTarget(hawk);
			}

		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;

			Send.ZC_NORMAL.PadRemoveEffect(pad, "F_archer_circling_ground");
			Send.ZC_NORMAL.PadUpdate(pad, false);
			PadRemoveBuff(pad, RelationType.All, 0, 0, BuffId.UC_Detected_Debuff);
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!creator.IsEnemy(initiator))
				return;

			if (initiator.IsBuffActive(BuffId.UC_Detected_Debuff))
				return;

			initiator.StartBuff(BuffId.UC_Detected_Debuff, skill.Level, 0f, pad.Trigger.RemainingLifeTime, creator);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var initiator = args.Initiator;

			initiator.StopBuff(BuffId.UC_Detected_Debuff);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			if (creator.IsDead)
			{
				pad.Destroy();
				return;
			}

			var enemies = pad.Trigger.GetAttackableEntities(creator);
			foreach (var enemy in enemies)
			{
				if (enemy.IsDead || enemy.IsBuffActive(BuffId.UC_Detected_Debuff))
					continue;

				enemy.StartBuff(BuffId.UC_Detected_Debuff, skill.Level, 0f, pad.Trigger.RemainingLifeTime, creator);
			}
		}
	}
}
