//--- GuiltineSin Script ----------------------------------------------------------
// Premium Item Scripts
//--- Description -----------------------------------------------------------
// Item scripts related to premium items, such as tokens.
//---------------------------------------------------------------------------

using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

public class PremiumItemScripts : GeneralScript
{
	[ScriptableFunction]
	public ItemTxResult SCR_USE_ITEM_PREMIUM_TOKEN(Character character, Item item, int[] numArgs)
	{
		var account = character.Connection.Account;

		// The duration is saved on the item's script data. NumArg1 is the number
		// of seconds. The purpose of NumArg2 is currently unknown.
		var timeSeconds = item.Data.Script.NumArg1;

		var addTime = TimeSpan.FromSeconds(timeSeconds);
		account.Premium.Token.Extend(addTime);

		if (account.Premium.CanUseBuff)
			character.StartBuff(BuffId.Premium_Token);

		Send.ZC_SEND_PREMIUM_STATE(character.Connection, account.Premium.Token);

		character.Inventory.Remove(item, 1, InventoryItemRemoveMsg.Used);

		return ItemTxResult.Okay;
	}
}
