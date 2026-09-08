using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiltineSin.Zone.Network;

namespace GuiltineSin.Zone.World.Actors.Effects
{
	public class ReviveEffect : Effect
	{
		public override void ShowEffect(IZoneConnection conn, IActor actor)
		{
			Send.ZC_NORMAL.Revive(conn, actor);
		}
	}
}
