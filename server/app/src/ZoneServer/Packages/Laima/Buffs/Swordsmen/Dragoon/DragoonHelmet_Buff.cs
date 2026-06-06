using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Zone.Buffs.Base;
using Melia.Zone.Network;
using Melia.Zone.Scripting.ScriptableEvents;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.World.Actors;
using Melia.Zone.World.Actors.Characters;
using Melia.Zone.World.Actors.Monsters;

namespace Melia.Zone.Buffs.Handlers.Swordsmen.Dragoon
{
	[Package("laima")]
	[BuffHandler(BuffId.DragoonHelmet_Buff)]
	public class DragoonHelmet_BuffOverride : BuffHandler
	{
		private const float MoveSpeedBonus = 7f;
		private const float DragoonSkillDamageBonus = 0.40f;
		private const float RaceDamageBonus = 0.60f;
		private const float BeastDemonDamageReduction = 0.30f;
		private const int DragoonHelmetItemId = 10008;
		private const int HpDrainIntervalMs = 15000;
		private const float HpDrainRatio = 0.05f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, MoveSpeedBonus);
			buff.SetUpdateTime(HpDrainIntervalMs);

			if (buff.Target is Character character)
			{
				Send.ZC_NORMAL.UpdateCharacterLook(character, DragoonHelmetItemId, EquipSlot.Helmet);
				RefreshSkillState(character);
			}
		}

		public override void WhileActive(Buff buff)
		{
			if (buff.Target is not Character character)
				return;

			var minHp = Math.Max(1f, character.MaxHp * HpDrainRatio);
			if (character.Hp <= minHp)
			{
				character.StopBuff(buff.Id);
				return;
			}

			var drain = character.MaxHp * HpDrainRatio;
			if (character.Hp - drain <= minHp)
			{
				character.ModifyHp(minHp - character.Hp);
				character.StopBuff(buff.Id);
				return;
			}

			character.ModifyHp(-drain);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);

			if (buff.Target is Character character)
			{
				var equippedHelmet = character.Inventory.GetEquip(EquipSlot.Helmet);
				Send.ZC_NORMAL.UpdateCharacterLook(character, equippedHelmet?.Id ?? 0, EquipSlot.Helmet);
				RefreshSkillState(character);
			}
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragoonHelmet_Buff)]
		public void OnAttackBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.IsBuffActive(BuffId.DragoonHelmet_Buff) || !IsDragoonSkill(skill))
				return;

			modifier.FinalDamageMultiplier *= 1f + DragoonSkillDamageBonus;
			if (IsBeastOrDemon(target))
				modifier.FinalDamageMultiplier *= 1f + RaceDamageBonus;

			modifier.Unblockable = true;
		}

		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragoonHelmet_Buff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (target.IsBuffActive(BuffId.DragoonHelmet_Buff) && IsBeastOrDemon(attacker))
				modifier.DamageMultiplier *= 1f - BeastDemonDamageReduction;
		}

		private static bool IsDragoonSkill(Skill skill)
		{
			return skill.Id == SkillId.Dragoon_Dragontooth
				|| skill.Id == SkillId.Dragoon_Serpentine
				|| skill.Id == SkillId.Dragoon_Gae_Bulg
				|| skill.Id == SkillId.Dragoon_Dragon_Soar
				|| skill.Id == SkillId.Dragoon_Dethrone
				|| skill.Id == SkillId.Dragoon_DragonFear
				|| skill.Id == SkillId.Dragoon_DragonFall
				|| skill.Id == SkillId.Dragoon_DragoonHelmet;
		}

		private static bool IsBeastOrDemon(ICombatEntity entity)
		{
			return entity is Mob && (entity.Race == RaceType.Widling || entity.Race == RaceType.Velnias);
		}

		private static void RefreshSkillState(Character character)
		{
			Send.ZC_NORMAL.SetSkillsProperties(character.Connection);
			Send.ZC_NORMAL.UpdateSkillUI(character);
		}
	}

	[Package("laima")]
	[BuffHandler(BuffId.DragoonHelmet_Debuff)]
	public class DragoonHelmet_DebuffOverride : BuffHandler
	{
		[CombatCalcModifier(CombatCalcPhase.BeforeCalc, BuffId.DragoonHelmet_Debuff)]
		public void OnDefenseBeforeCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (target.IsBuffActive(BuffId.DragoonHelmet_Debuff))
				modifier.DamageMultiplier *= 1.25f;
		}
	}
}
