using System;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Events.Arguments
{
	/// <summary>
	/// Arguments for events related to a monster.
	/// </summary>
	/// <param name="monster"></param>
	public class MonsterEventArgs(IMonster monster) : EventArgs
	{
		/// <summary>
		/// Returns the monster associated with the event.
		/// </summary>
		public IMonster Monster { get; } = monster;
	}
}
