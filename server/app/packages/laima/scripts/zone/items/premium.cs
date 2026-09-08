//--- GuiltineSin Script ----------------------------------------------------------
// Premium Items (Repair Kit...)
//--- Description -----------------------------------------------------------
// Item-related scripts that are defined as Premium group items.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Items;
using Yggdrasil.Logging;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Scripting.Shortcuts;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;

public class PremiumItemScripts : GeneralScript
{
	[ScriptableFunction]
	public ItemUseResult PREMIUM_REPAIR(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		foreach (var equipItem in character.Inventory.GetEquip().Values)
			equipItem.ModifyDurability(character, (int)item.Data.Script.NumArg1);
		return ItemUseResult.Okay;
	}

	[ScriptableFunction]
	public ItemUseResult SCR_USE_GOLDMORU_BOX(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		character.AddItem(ItemId.Moru_Gold, (int)numArg1);
		return ItemUseResult.Okay;
	}

	[ScriptableFunction]
	public ItemUseResult SCR_USE_EXTEND_ACCOUNT_WAREHOUSE(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		character.ModifyAccountProperty(PropertyName.AccountWareHouseExtend, 1);
		character.AddonMessage(AddonMessage.ACCOUNT_WAREHOUSE_ITEM_LIST);
		character.AddonMessage(AddonMessage.ACCOUNT_UPDATE);
		return ItemUseResult.Okay;
	}

	[ScriptableFunction]
	public ItemUseResult SCR_USE_FREE_EXTEND_ACCOUNT_WAREHOUSE(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		var amount = (int)numArg2;
		if (amount <= 0) amount = 1;

		character.ModifyAccountProperty(PropertyName.AccountWareHouseExtendByItem, amount);

		character.AddonMessage(AddonMessage.ACCOUNT_WAREHOUSE_ITEM_LIST);
		character.AddonMessage(AddonMessage.ACCOUNT_UPDATE);

		if (amount == 1)
			character.AddonMessage(AddonMessage.NOTICE_Dm_Scroll, ScpArgMsg("ACCOUNT_UPDATE1"), 5);
		else if (amount == 2)
			character.AddonMessage(AddonMessage.NOTICE_Dm_Scroll, ScpArgMsg("ACCOUNT_UPDATE2"), 5);

		return ItemUseResult.Okay;
	}
}
