using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Pads.Handlers;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;

namespace GuiltineSin.Zone.Pads.HandlersOverride.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Corsair Jolly Roger pad.
	/// Provides party buffs and enemy debuffs within range.
	/// </summary>
	[Package("laima")]
	[PadHandler(PadName.Corsair_JollyRoger)]
	public class Corsair_JollyRogerOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, ILeavePadHandler, IUpdatePadHandler
	{
		private const float PadRange = 150f;
		private const int UpdateIntervalMs = 100;
		private const int PadLifetimeMs = 30000;

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(PadRange);
			pad.SetUpdateInterval(UpdateIntervalMs);
			this.CreateFlagMonster(pad);
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(PadLifetimeMs);
		}

		private void CreateFlagMonster(Pad pad)
		{
			var monster = PadCreateMonster(pad, "JollyRoger_Black", pad.Position, 0f, 1, 0f, "", "None", 4.5f, true, "", "None");
			var mob = monster as Mob;
			mob.MonsterType = RelationType.Friendly;
			mob.Faction = FactionType.Neutral;
			mob.StartBuff(BuffId.Invincible);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, false);
			PadRemoveBuff(pad, RelationType.All, 0, 0, BuffId.JollyRoger_Buff);
			PadRemoveBuff(pad, RelationType.All, 0, 0, BuffId.JollyRoger_Enemy_Debuff);
			PadRemoveBuff(pad, RelationType.All, 0, 0, BuffId.Looting_Buff);
		}

		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			PadTargetBuff(pad, initiator, RelationType.Party, 0, 0, BuffId.JollyRoger_Buff, skill.Level, 0, 0, 1, 100, false);
			PadTargetBuff(pad, initiator, RelationType.Enemy, 0, 0, BuffId.JollyRoger_Enemy_Debuff, skill.Level, 0, 0, 1, 100, false);
			PadTargetBuff(pad, initiator, RelationType.Party, 0, 0, BuffId.Looting_Buff, skill.Level, 0, 0, 1, 100, false);
		}

		public void Left(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var initiator = args.Initiator;

			PadTargetBuffRemove(pad, initiator, RelationType.All, 0, 0, BuffId.JollyRoger_Buff, false);
			PadTargetBuffRemove(pad, initiator, RelationType.All, 0, 0, BuffId.JollyRoger_Enemy_Debuff, false);
			PadTargetBuffRemove(pad, initiator, RelationType.All, 0, 0, BuffId.Looting_Buff, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;
		}
	}
}
