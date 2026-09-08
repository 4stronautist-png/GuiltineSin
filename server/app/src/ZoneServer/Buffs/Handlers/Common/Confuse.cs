using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handler for the Confuse debuff, which confuses and immobilizes targets.
	/// </summary>
	[BuffHandler(BuffId.Confuse)]
	public class Confuse : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			Send.ZC_SHOW_EMOTICON(target, "I_emo_confuse", buff.Duration);
			target.AddState(StateType.Stunned, buff.Duration);
		}

		public override void OnExtend(Buff buff)
		{
			var target = buff.Target;

			target.AddState(StateType.Stunned, buff.Duration);
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
