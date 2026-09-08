using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Give ride to master, None..
	/// </summary>
	[BuffHandler(BuffId.TakingOwner)]
	public class TakingOwner : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = (Character)buff.Caster;
			var target = (Companion)buff.Target;

			target.StopMove();
			target.Position = caster.Position;

			if (target.Components.TryGet<CombatComponent>(out var combat))
				combat.InterruptCasting();

			if (target.Components.TryGet<AiComponent>(out var aiComponent))
				aiComponent.Script.Suspended = true;
			target.IsRiding = true;
			target.SetHittable(false);
			Send.ZC_NORMAL.RidePet(caster, target);
		}

		public override void OnEnd(Buff buff)
		{
			var caster = (Character)buff.Caster;
			var target = (Companion)buff.Target;

			if (target.Components.TryGet<AiComponent>(out var aiComponent))
				aiComponent.Script.Suspended = false;
			target.IsRiding = false;
			target.SetHittable(true);
			target.Position = caster.Position;
			Send.ZC_NORMAL.RidePet(caster, target);
		}
	}
}
