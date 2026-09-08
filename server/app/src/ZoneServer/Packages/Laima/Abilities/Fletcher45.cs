using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Handlers.Archers.Fletcher;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Abilities.Handlers
{
	[Package("laima")]
	[AbilityHandler(AbilityId.Fletcher45)]
	public class Fletcher45Override : IAbilityPropertyHandler
	{
		public void OnActivate(Ability ability, Character character)
		{
		}

		public void OnDeactivate(Ability ability, Character character)
		{
			if (!character.TryGetSkill(SkillId.Fletcher_CrossFire, out var skill) || skill.IsOnCooldown)
				return;

			if (!Fletcher_FletcherArrowShotOverride.HasQuiverSpace(character, Fletcher_FletcherArrowShotOverride.CrossFireCost))
				return;

			var cooldown = character.StartCooldown(skill.Data.CooldownGroup, skill.Properties.CoolDown);
			if (cooldown != null)
				cooldown.OnCooldownChanged = () => skill.OnCooldownChanged?.Invoke();

			character.StartBuff(BuffId.Fletcher_CrossFire_Buff, TimeSpan.Zero);
		}
	}
}
