using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Pads.Handlers
{
	/// <summary>
	/// Pad handler for Falconer's Aiming skill.
	/// Follows the hawk companion. Applies Aiming_Buff to enemies
	/// in range every tick, increasing their effective hit radius
	/// for AoE attacks.
	/// </summary>
	[Package("laima")]
	[PadHandler(PadName.Falconer_Aiming)]
	public class Falconer_AimingOverride : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		private const float AimingRange = 100f;
		private const int UpdateIntervalMs = 1000;
		private const int BuffDurationMs = 1500;

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetRange(AimingRange);
			pad.SetUpdateInterval(UpdateIntervalMs);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;

			Send.ZC_NORMAL.PadUpdate(pad, false);
			PadRemoveBuff(pad, RelationType.All, 0, 0, BuffId.Aiming_Buff);
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

			var buffDuration = TimeSpan.FromMilliseconds(BuffDurationMs);
			var enemies = pad.Trigger.GetAttackableEntities(creator);

			foreach (var enemy in enemies)
			{
				if (enemy.IsDead)
					continue;

				enemy.StartBuff(BuffId.Aiming_Buff, skill.Level, 0f, buffDuration, creator, skill.Id);
			}
		}
	}
}
