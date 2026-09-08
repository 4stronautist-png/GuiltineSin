using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Petrify, Petrified..
	/// </summary>
	[BuffHandler(BuffId.UC_petrify)]
	public class Mon_Petrify: BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = buff.Caster;
			var target = buff.Target;

			Send.ZC_SHOW_EMOTICON(target, "I_emo_petrify", buff.Duration);
			Send.ZC_NORMAL.PlayTextEffect(target, caster, AnimationName.ShowBuffText, (float)buff.Id, null);
			Send.ZC_PLAY_SOUND(target, "skl_eff_debuff_stone");

			buff.Target.AddState(StateType.Petrified);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			buff.Target.RemoveState(StateType.Petrified);
		}
	}
}
