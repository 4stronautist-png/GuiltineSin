using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Elite Subordinate Buff, Monster that follows the leader of the group..
	/// </summary>
	[BuffHandler(BuffId.EliteMonsterSummonBuff)]
	public class EliteMonsterSummonBuff : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}
	}
}
