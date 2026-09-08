using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using static GuiltineSin.Shared.Network.NormalOp;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Mass Heal, HP continuously restored..
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.MassHeal_Dot_Buff)]
	public class MassHeal_Dot_BuffOverride : BuffHandler
	{
		public override void WhileActive(Buff buff)
		{
			var healAmount = buff.NumArg2;

			buff.Target.Heal(healAmount, 0);
		}
	}
}
