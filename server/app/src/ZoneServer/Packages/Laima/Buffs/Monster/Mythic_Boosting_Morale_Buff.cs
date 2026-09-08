using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers.Monster;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers.Laima.Monster
{
	/// <summary>
	/// Handler for Mythic_Boosting_Morale_Buff.
	/// Spawns minions and periodically applies ATK/DEF boost
	/// to nearby allied monsters. Respawns minions if count drops below 5.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Mythic_Boosting_Morale_Buff)]
	public class Mythic_Boosting_Morale_BuffOverride : BuffHandler
	{
		private const string MinionHandlesVar = "GuiltineSin.Mythic.Minions";
		private const string StatsAppliedVar = "GuiltineSin.Mythic.StatsApplied";
		private const int TargetMinionCount = 5;
		private const float MoraleRange = 150f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target is not Mob monster)
				return;

			MythicBuffHelper.ApplyMythicStats(monster);
			buff.Vars.Set(StatsAppliedVar, true);
			buff.Vars.Set(MinionHandlesVar, new List<int>());
			buff.SetUpdateTime(2000);
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.Target is not Mob monster || monster.Map == null || monster.IsDead)
				return;

			// Maintain minion count
			if (!buff.Vars.TryGet<List<int>>(MinionHandlesVar, out var minionHandles))
			{
				minionHandles = new List<int>();
				buff.Vars.Set(MinionHandlesVar, minionHandles);
			}

			MythicBuffHelper.MaintainMinions(buff, monster, minionHandles, TargetMinionCount);

			// Apply morale buff to nearby monsters
			var range = MoraleRange;

			var nearbyMobs = monster.Map.GetActorsInRange<Mob>(monster.Position, range, m => !m.IsDead && m.Handle != monster.Handle);

			foreach (var mob in nearbyMobs)
			{
				if (!mob.IsBuffActive(BuffId.Mythic_Boosting_Morale_Atk_Buff))
					mob.StartBuff(BuffId.Mythic_Boosting_Morale_Atk_Buff, 1, 0, TimeSpan.FromSeconds(5), monster);
			}
		}

		public override void OnEnd(Buff buff)
		{
			if (buff.Target is not Mob monster || monster.Map == null)
				return;

			var range = MoraleRange;

			var nearbyMobs = monster.Map.GetActorsInRange<Mob>(monster.Position, range);
			foreach (var mob in nearbyMobs)
			{
				mob.StopBuff(BuffId.Mythic_Boosting_Morale_Atk_Buff);
			}
		}
	}
}
