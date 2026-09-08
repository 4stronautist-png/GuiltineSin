using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handle for Silence, prevents attacks
	/// (Should we prevent magic attacks only instead? that'd be cool, albeit
	/// not official behaviour).
	/// </summary>
	[BuffHandler(BuffId.Common_Silence, BuffId.UC_silence)]
	public class Common_Silence : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = buff.Caster;
			var target = buff.Target;

			Send.ZC_SHOW_EMOTICON(target, "I_emo_silence", buff.Duration);
			buff.Target.AddState(StateType.Silenced);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.RemoveState(StateType.Silenced);
		}
	}
}
