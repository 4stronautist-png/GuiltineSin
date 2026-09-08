using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters.Components;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Swordsman.Peltasta
{
	/// <summary>
	/// Handle for the Hard Shield Buff, which increases defense
	/// based on the defense power of the equipped shield
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.HardShield_Buff)]
	public class HardShield_BuffOverride : BuffHandler
	{
		private const float DefMultiplierPerLevel = 0.2f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			AddPropertyModifier(buff, buff.Target, PropertyName.DEF_BM, this.GetDefBonus(buff));
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.DEF_BM);
		}

		private float GetDefBonus(Buff buff)
		{
			var shieldDef = 0f;
			var byAbility = 1f;

			if (buff.Caster.Components.TryGet<InventoryComponent>(out var inv))
			{
				var lhItem = inv.GetItem(EquipSlot.LeftHand);
				if (lhItem.Data.EquipType1 == EquipType.Shield)
					shieldDef = lhItem.Data.Def;
			}

			if (buff.Caster is ICombatEntity casterEntity && casterEntity.TryGetSkill(buff.SkillId, out var skill))
			{
				var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
				byAbility += SCR_Get_AbilityReinforceRate(skill);
			}

			return shieldDef * DefMultiplierPerLevel * buff.NumArg1 * byAbility;
		}
	}
}
