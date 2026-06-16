using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Zone.Buffs;
using Melia.Zone.Buffs.Base;
using Melia.Zone.Network;
using Melia.Zone.Scripting.ScriptableEvents;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Scouts.BlitzHunter;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.CombatEntities.Components;

namespace Melia.Zone.Buffs.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[BuffHandler(BuffId.Blitzkrieg_Buff)]
		public class Blitzkrieg_BuffOverride : BuffHandler
		{
			private const string BlitzkriegAuraEffect = "BodyAura_Electric_Blue_01";
			private const string BlitzkriegOverwhelmingEffect = "BodyAura_PowerOverwhelming_Blue_01";
			private const string BlitzkriegPowerUpEffect = "PowerUp_Electric_Blue_02";

			public override void OnActivate(Buff buff, ActivationType activationType)
			{
				if (!buff.Target.Components.TryGet<CooldownComponent>(out var cooldownComponent))
					return;

				foreach (var skillId in BlitzHunterSkillHelper.GetBlitzkriegAffectedSkills(buff.Target))
				{
					if (!buff.Target.TryGetSkill(skillId, out var skill))
						continue;

					if (BlitzHunterSkillHelper.TryGetBlitzkriegReducedCooldown(skillId, out var reducedCooldown) && skill.Data.CooldownTime > reducedCooldown)
						cooldownComponent.AddReduction(skillId, skill.Data.CooldownTime - reducedCooldown);

					// Retail clears the consumed skill's current stack/cooldown state when Blitzkrieg starts.
					skill.StartCooldown(TimeSpan.Zero);
				}

				BlitzHunterSkillHelper.InvalidateBlitzkriegSkillProperties(buff.Target);
				BlitzHunterSkillHelper.SendBlitzkriegSkillState(buff.Target);
			}

		public override void OnEnd(Buff buff)
		{
			if (!buff.Target.Components.TryGet<CooldownComponent>(out var cooldownComponent))
				return;

				foreach (var skillId in BlitzHunterSkillHelper.GetBlitzkriegAffectedSkills(buff.Target))
					cooldownComponent.ResetReduction(skillId);

				Send.ZC_NORMAL.RemoveEffectByName(buff.Target, BlitzkriegAuraEffect, true);
				Send.ZC_NORMAL.RemoveEffectByName(buff.Target, BlitzkriegOverwhelmingEffect, true);
				Send.ZC_NORMAL.RemoveEffectByName(buff.Target, BlitzkriegPowerUpEffect, true);

				BlitzHunterSkillHelper.InvalidateBlitzkriegSkillProperties(buff.Target);
				BlitzHunterSkillHelper.SendBlitzkriegSkillState(buff.Target);
			}

		[CombatCalcModifier(CombatCalcPhase.BeforeBonuses, BuffId.Blitzkrieg_Buff)]
		public void OnBeforeBonuses(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!BlitzHunterSkillHelper.IsBlitzHunterDamageSkill(skill.Id))
				return;

			if (!attacker.TryGetBuff(BuffId.Blitzkrieg_Buff, out var buff))
				return;

			var damageBonus = 0.02f * buff.NumArg1;
			if (attacker.IsAbilityActive(AbilityId.BlitzHunter9))
				damageBonus += 0.05f;

			modifier.FinalDamageMultiplier *= 1f + damageBonus;
		}
	}
}
