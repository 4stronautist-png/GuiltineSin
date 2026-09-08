using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Clerics.Dievdirbys
{
	/// <summary>
	/// Handler override for CarveLaima_Buff, which forces a recalculation
	/// of skill cooldowns when it is applied and removed.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.CarveLaima_Buff)]
	public class CarveLaima_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target.Components.TryGet<SkillComponent>(out var skillComponent))
			{
				skillComponent.InvalidateAll();
			}
		}

		public override void OnEnd(Buff buff)
		{
			if (buff.Target.Components.TryGet<SkillComponent>(out var skillComponent))
			{
				skillComponent.InvalidateAll();
			}
		}
	}
}
