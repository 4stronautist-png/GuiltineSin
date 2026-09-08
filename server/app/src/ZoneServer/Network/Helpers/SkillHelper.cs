using GuiltineSin.Zone.Skills;
using GuiltineSin.Shared.Network;
using GuiltineSin.Shared.Network.Helpers;

namespace GuiltineSin.Zone.Network.Helpers
{
	public static class SkillHelper
	{
		/// <summary>
		/// Adds skill data to packet.
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="skill"></param>
		public static void AddSkill(this Packet packet, Skill skill)
		{
			var propertyList = skill.Properties.GetAll();
			var propertiesSize = propertyList.GetByteCount();

			packet.PutLong(skill.ObjectId);
			packet.PutInt((int)skill.Id);
			packet.PutShort(propertiesSize);
			packet.PutEmptyBin(2); // alignment
			packet.PutInt(0);
			packet.PutShort(0);
			packet.PutEmptyBin(2); // alignment
			packet.AddProperties(propertyList);
		}
	}
}
