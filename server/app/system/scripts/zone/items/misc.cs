//--- GuiltineSin Script ----------------------------------------------------------
// Miscellanous Item Scripts
//--- Description -----------------------------------------------------------
// Item scripts that don't require dedicated files.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

public class MiscItemScripts : GeneralScript
{
	[ScriptableFunction]
	public ItemUseResult SCR_UES_ITEM_BOOK(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		var bookName = strArg;
		Send.ZC_NORMAL.OpenBook(character, bookName);

		return ItemUseResult.OkayNotConsumed;
	}
}
