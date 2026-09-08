using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Dievdirbys
{
	/// <summary>
	/// Handle for CarveZemina_Buff, which reduces SP consumption of skills
	/// by 20% + 2% per skill level.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.CarveZemina_Buff)]
	public class Dievdirbys_CarveZemina_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			// Store skill level in buff for SP calculation
			// buff.NumArg1 already contains the skill level from the pad handler

			// Force skill component to recalculate SP costs
			if (buff.Target.Components.TryGet<SkillComponent>(out var skillComponent))
			{
				skillComponent.InvalidateAll();
			}
		}

		public override void OnEnd(Buff buff)
		{
			// Force skill component to recalculate SP costs when buff ends
			if (buff.Target.Components.TryGet<SkillComponent>(out var skillComponent))
			{
				skillComponent.InvalidateAll();
			}
		}
	}
}
