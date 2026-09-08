using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using System;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Scouts.Rogue
{
	/// <summary>
	/// Handler for the Burrow buff. Hides underground, becoming
	/// untargetable and invincible with fixed movement speed.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Burrow_Rogue)]
	public class Burrow_RogueOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			Send.ZC_NORMAL.ApplyBuff(target, "Burrow_Rogue", SkillId.Rogue_Burrow, true);
			Send.ZC_NORMAL.Skill_10D(target, SkillId.Rogue_Burrow);

			AddPropertyModifier(buff, target, PropertyName.FIXMSPD_BM, 15);
			AddPropertyModifier(buff, target, PropertyName.Jumpable, -1);

			target.Lock(LockType.GetDamaged);
			target.SetHideFromMon(true);

			buff.SetUpdateTime(1000);

			if (target is Character character)
				character.InvalidateProperties(PropertyName.MSPD);

			Send.ZC_NORMAL.SetActorShadow(target, false);
		}

		public override void WhileActive(Buff buff)
		{
			var target = buff.Target;
			var skillLevel = (int)buff.NumArg1;
			var healRate = 0.05f + 0.005f * skillLevel;

			if (target.TryGetSkill(buff.SkillId, out var skill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				healRate *= 1f + SCR_Get_AbilityReinforceRate(skill);
			}

			var healAmount = target.MaxHp * healRate;

			target.Heal(healAmount, 0);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;
			var caster = buff.Caster;

			Send.ZC_NORMAL.RemoveBuff(target, "Burrow_Rogue");
			Send.ZC_NORMAL.Skill_10E(target);
			Send.ZC_NORMAL.SetActorShadow(target, true);

			RemovePropertyModifier(buff, target, PropertyName.FIXMSPD_BM);
			RemovePropertyModifier(buff, target, PropertyName.Jumpable);

			target.Unlock(LockType.GetDamaged);
			target.SetHideFromMon(false);

			if (target is Character character)
			{
				if (!character.TryGetSkill(SkillId.Rogue_Burrow, out var skill))
					return;

				var targets = character.SelectObjectNear(character, 30, RelationType.Enemy);
				foreach (var enemy in targets)
				{
					enemy.StartBuff(BuffId.UC_silence, TimeSpan.FromSeconds(3), character);

					if (!character.IsAbilityActive(AbilityId.Rogue8))
						continue;

					var skillHitResult = SCR_SkillHit(character, enemy, skill);
					enemy.TakeDamage(skillHitResult.Damage, character);

					var hit = new HitInfo(character, enemy, skill, skillHitResult.Damage, skillHitResult.Result);
					Send.ZC_HIT_INFO(hit.Attacker, hit.Target, hit);
				}
			}
		}
	}
}
