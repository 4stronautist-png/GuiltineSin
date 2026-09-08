using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Makes entity invincible
	/// </summary>
	[BuffHandler(BuffId.Invincible)]
	public class Invincible_Buff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.SetTempVar(PropertyName.HitProof, 1);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.SetTempVar(PropertyName.HitProof, 0);
		}
	}
}
