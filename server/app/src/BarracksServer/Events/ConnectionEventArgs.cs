using System;
using GuiltineSin.Barracks.Network;

namespace GuiltineSin.Barracks.Events
{
	/// <summary>
	/// Event arguments related to a connection.
	/// </summary>
	public class ConnectionEventArgs : EventArgs
	{
		/// <summary>
		/// Returns the related connection.
		/// </summary>
		public IBarracksConnection Connection { get; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="connection"></param>
		public ConnectionEventArgs(IBarracksConnection connection)
		{
			this.Connection = connection;
		}
	}
}
