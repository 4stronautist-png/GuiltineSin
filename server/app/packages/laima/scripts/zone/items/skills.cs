//--- GuiltineSin Script ----------------------------------------------------------
// Doll Items
//--- Description -----------------------------------------------------------
// Item scripts that add and remove skills on equipping.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

public class SkillItemScript : GeneralScript
{

	[ScriptableFunction("SCP_ON_EQUIP_ITEM_SKILL")]
	public ItemEquipResult SCP_ON_EQUIP_ITEM_SKILL(Character character, Item item, EquipSlot equipSlot)
	{
		var skillClassname = item.Data.Script.StrArg2;

		if (ZoneServer.Instance.Data.SkillDb.TryFind(skillClassname, out var skillData))
		{
			if (!character.HasSkill(skillData.Id))
				character.Skills.AddSilent(new Skill(character, skillData.Id, 1, isEquipSkill: true));
		}

		return ItemEquipResult.Okay;
	}

	[ScriptableFunction("SCP_ON_UNEQUIP_ITEM_SKILL")]
	public ItemUnequipResult SCP_ON_UNEQUIP_ITEM_SKILL(Character character, Item item, EquipSlot equipSlot)
	{
		var skillClassname = item.Data.Script.StrArg2;

		if (ZoneServer.Instance.Data.SkillDb.TryFind(skillClassname, out var skillData))
		{
			if (character.TryGetSkill(skillData.Id, out var skill) && skill.IsEquipSkill)
				character.Skills.RemoveSilent(skillData.Id);
		}

		return ItemUnequipResult.Okay;
	}
}
