using GuiltineSin.Shared.Network;

namespace GuiltineSin.Social.Network
{
	public static partial class Send
	{
		/// <summary>
		/// Informs client about successful login.
		/// </summary>
		/// <param name="conn"></param>
		public static void SC_LOGIN_OK(ISocialConnection conn)
		{
			using var packet = Packet.Rent(Op.SC_LOGIN_OK);

			conn.Send(packet);
		}
	}
}
