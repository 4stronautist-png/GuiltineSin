using System.Collections.Generic;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Iron Hook buff on the caster.
	/// Locks attack and movement while holding the hook.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.IronHook)]
	public class IronHook_BuffOverride : BuffHandler
	{
		public override void OnEnd(Buff buff)
		{
			if (buff.Caster is not ICombatEntity caster)
				return;

			caster.Interrupt();

			if (buff.Vars.TryGet<List<ICombatEntity>>("GuiltineSin.IronHook.Targets", out var targets))
			{
				foreach (var target in targets)
				{
					Send.ZC_NORMAL.RemoveHookEffect(caster);
					Send.ZC_NORMAL.RemoveEffectByName(caster, "Warrior_Pull", true);

					target.RemoveBuff(BuffId.IronHooked);
				}
				buff.Vars.Remove("GuiltineSin.IronHook.Targets");
			}
		}
	}
}
