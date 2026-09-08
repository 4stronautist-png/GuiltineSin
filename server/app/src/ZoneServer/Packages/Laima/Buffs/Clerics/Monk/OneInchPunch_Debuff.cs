using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handle for the OneInchPunch_Debuff, which prevents natural SP regeneration.
	/// SP regen blocking is handled by the NoneRecoverableSP buff tag in
	/// SCR_Get_Character_RSP.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.OneInchPunch_Debuff)]
	public class OneInchPunch_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target is Character character)
			{
				character.Properties.Invalidate(PropertyName.RSP);
			}
		}

		public override void OnEnd(Buff buff)
		{
			if (buff.Target is Character character)
			{
				character.Properties.Invalidate(PropertyName.RSP);
			}
		}
	}
}
