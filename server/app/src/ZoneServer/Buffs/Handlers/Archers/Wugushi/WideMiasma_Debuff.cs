using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Archers.Wugushi
{
	/// <summary>
	/// Healing reduction applied by Wide Miasma.
	/// </summary>
	/// <remarks>
	/// NumArg1: Skill level
	/// NumArg2: Heal reduction percentage in thousands
	/// </remarks>
	[BuffHandler(BuffId.WideMiasma_Debuff)]
	public class WideMiasma_Debuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			Send.ZC_NORMAL.PlayTextEffect(buff.Target, buff.Caster, "SHOW_BUFF_TEXT", (float)BuffId.WideMiasma_Debuff, null);
			buff.NotifyUpdate();
		}
	}
}
