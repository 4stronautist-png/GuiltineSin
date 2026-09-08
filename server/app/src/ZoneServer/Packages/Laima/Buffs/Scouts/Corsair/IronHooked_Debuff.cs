using System.Collections.Generic;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Actors.Effects;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Iron Hooked debuff on the target.
	/// Locks attack and movement while hooked.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.IronHooked)]
	public class IronHooked_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			target.Lock(LockType.Attack);
			target.Lock(LockType.Movement);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.Unlock(LockType.Attack);
			buff.Target.Unlock(LockType.Movement);

			if (buff.Caster is not ICombatEntity caster)
				return;

			if (!caster.TryGetBuff(BuffId.IronHook, out var casterBuff))
				return;

			if (!casterBuff.Vars.TryGet<List<ICombatEntity>>("GuiltineSin.IronHook.Targets", out var targets))
				return;

			var allTargetsGone = true;
			foreach (var target in targets)
			{
				if (target != null && !target.IsDead)
				{
					allTargetsGone = false;
					break;
				}
			}

			if (allTargetsGone)
				caster.RemoveBuff(BuffId.IronHook);
		}
	}
}
