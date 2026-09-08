using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads.Handlers;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.Pads.HandlersOverride.Archers.Fletcher
{
	[Package("laima")]
	[PadHandler(PadName.Fletcher_CatenaChainArrow_PAD)]
	public class Fletcher_CatenaChainArrowOverride : ICreatePadHandler, IDestroyPadHandler, IUpdatePadHandler
	{
		private const float MaxLeashDistance = 150f;

		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			if (pad.Variables.TryGet<Npc>("GuiltineSin.Pad.CatenaAnchor", out var anchor))
				anchor.DisappearTime = DateTime.Now;

			creator.RemoveEffect("GuiltineSin.Skill.CatenaChainLink");
			creator.StopBuff(BuffId.Fletcher_CatenaChainArrow_Buff);

			var skill = pad.Skill;
			if (skill != null)
				skill.Vars.SetInt("GuiltineSin.Skill.CatenaPadHandle", 0);

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			var distance = creator.Position.Get2DDistance(pad.Position);
			if (distance >= MaxLeashDistance)
			{
				Send.ZC_NORMAL.PadUpdate(pad, false);
				creator.Map.RemovePad(pad);
			}
		}
	}
}
