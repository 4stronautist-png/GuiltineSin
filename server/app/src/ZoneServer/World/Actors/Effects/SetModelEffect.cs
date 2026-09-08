using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.World.Actors.Effects
{
	internal class SetModelEffect : Effect
	{
		public string XACHeadName { get; }
		public int ModelId { get; }
		public SetModelEffect(string xacHeadName, int modelId = 0)
		{
			this.XACHeadName = xacHeadName;
			this.ModelId = modelId;
		}

		public override void ShowEffect(IZoneConnection conn, IActor actor)
		{
			Send.ZC_NORMAL.PadSetModel(conn, actor, this.XACHeadName, this.ModelId);
		}
	}
}
