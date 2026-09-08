using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Petrify, Petrified..
	/// </summary>
	[BuffHandler(BuffId.Petrification)]
	public class Petrification : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = buff.Caster;
			var target = buff.Target;

			Send.ZC_SHOW_EMOTICON(target, "I_emo_petrify", buff.Duration);
			Send.ZC_PLAY_SOUND(target, "skl_eff_debuff_stone");

			if (target is Character character)
				AddPropertyModifier(buff, character, PropertyName.Jumpable, -1);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			if (target is Character character)
				RemovePropertyModifier(buff, character, PropertyName.Jumpable);
		}
	}
}
