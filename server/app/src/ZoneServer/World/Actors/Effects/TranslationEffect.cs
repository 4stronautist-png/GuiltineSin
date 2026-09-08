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
	internal class TranslationEffect : Effect
	{
		public int PadHandle { get; }
		public float Height { get; }
		public TranslationEffect(int padHandle, float height = 0)
		{
			this.PadHandle = padHandle;
			this.Height = height;
		}

		public override void ShowEffect(IZoneConnection conn, IActor actor)
		{
			if (actor.Map.TryGetPad(this.PadHandle, out var pad))
				Send.ZC_NORMAL.PadSetMonsterAltitude(conn, pad, actor, this.Height);
		}
	}
}
