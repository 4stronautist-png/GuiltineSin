using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Assassin
{
	/// <summary>
	/// Buff handler for Behead: Bleeding, which deals damage in regular intervals.
	/// </summary>
	[BuffHandler(BuffId.Behead_Debuff)]
	public class Behead_Debuff : BuffHandler
	{
		public override void WhileActive(Buff buff)
		{
			var attacker = buff.Caster;
			var target = buff.Target;
			var damage = buff.NumArg2;

			target.TakeSimpleHit(damage, attacker, SkillId.Assassin_Behead_DOT);
		}
	}
}
