using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Jolly Roger Buff.
	/// Combo activated by attacking enemies. Fever buff initiated after 100 combos.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.JollyRoger_Buff)]
	public class JollyRoger_BuffOverride : BuffHandler
	{
		public override void OnEnd(Buff buff)
		{
			if (buff.Caster is not Character caster)
				return;

			if (buff.Target != buff.Caster)
				return;

			caster.Variables.Temp.Remove("GuiltineSin.Buff.JollyRoger");
			caster.Variables.Temp.Remove("GuiltineSin.Buff.JollyRoger.FeverStartTime");
		}
	}
}
