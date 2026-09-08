using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Zone.Buffs;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Scouts.BlitzHunter;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.BlitzHunter
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
