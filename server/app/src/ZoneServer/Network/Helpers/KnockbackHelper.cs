using GuiltineSin.Shared.Network;
using GuiltineSin.Shared.Network.Helpers;
using GuiltineSin.Zone.Skills.Combat;

namespace GuiltineSin.Zone.Network.Helpers
{
	public static class KnockbackHelper
	{
		/// <summary>
		/// Adds information about the knock back to the packet.
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="knockBackInfo"></param>
		public static void AddKnockbackInfo(this Packet packet, KnockBackInfo knockBackInfo)
		{
			packet.PutPosition(knockBackInfo.FromPosition);
			packet.PutPosition(knockBackInfo.ToPosition);
			packet.PutInt(knockBackInfo.Velocity);
			packet.PutInt(knockBackInfo.HAngle);
			packet.PutInt(knockBackInfo.VAngle);
			packet.PutInt(knockBackInfo.BounceCount);
			packet.PutShort((short)knockBackInfo.Time.TotalMilliseconds);
			packet.PutShort(0);
			packet.PutFloat(knockBackInfo.Speed);
			packet.PutFloat(knockBackInfo.VPow);
			packet.PutInt(0);
			packet.PutByte(0);
			packet.PutByte(0);
			if (knockBackInfo.HitType == Shared.Game.Const.KnockBackType.KnockDown)
			{
				packet.PutByte(40);
				packet.PutByte(64);
			}
			else
			{
				packet.PutByte(0);
				packet.PutByte(0);
			}

		}

		public static void AddKnockdownInfo(this Packet packet, KnockBackInfo knockdownInfo)
		{
			packet.AddKnockbackInfo(knockdownInfo);
			packet.PutByte((byte)knockdownInfo.HitType);
		}
	}
}
