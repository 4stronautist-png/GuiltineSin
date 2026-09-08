using System;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using Yggdrasil.Scheduling;

namespace GuiltineSin.Zone.World
{
	// Stub: Battles system was removed during Laima merge.
	public class BattleManager : IUpdateable
	{
		public void StartBattle(Character initiator, ICombatEntity target)
		{
			// No-op: Battles not available
		}

		public void Resume(Character character)
		{
			// No-op: Battles not available
		}

		public void EndBattle(Character character)
		{
			// No-op: Battles not available
		}

		public void ForceEndBattle(Character character)
		{
			// No-op: Battles not available
		}

		public void Update(TimeSpan elapsed)
		{
			// No-op: Battles not available
		}
	}
}
