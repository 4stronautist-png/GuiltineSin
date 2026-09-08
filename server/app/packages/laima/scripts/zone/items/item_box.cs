//--- GuiltineSin Script ----------------------------------------------------------
// Item Boxes (Costume box)
//--- Description -----------------------------------------------------------
// Item scripts that adds specific items to the inventory on use.
//---------------------------------------------------------------------------

using System;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class ItemBoxScript : GeneralScript
{
	[ScriptableFunction]
	public ItemUseResult SCR_USE_GIVE_ITEM_COSTUME_BOX(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		var items = ParseItems(strArg);

		foreach (var parsedItem in items)
			character.AddItem(parsedItem.Key, parsedItem.Value);

		return ItemUseResult.Okay;
	}

	[ScriptableFunction]
	public ItemUseResult SCR_USE_ITEM_ADD_CHAT_BALLOON(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		// SCR_USE_ITEM_ADD_CHAT_BALLOON("Balloon_0006", 7776000, 0)
		if (!int.TryParse(strArg.Replace("Balloon_00", ""), out var balloonId))
			return ItemUseResult.Fail;
		character.ChatBalloon = balloonId;
		character.ChatBalloonExpiration = DateTime.Now.AddSeconds(7776000);
		Send.ZC_SET_CHATBALLOON_SKIN(character);

		return ItemUseResult.Okay;
	}
}
