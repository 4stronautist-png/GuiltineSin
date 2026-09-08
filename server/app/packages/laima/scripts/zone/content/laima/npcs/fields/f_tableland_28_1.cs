//--- GuiltineSin Script ----------------------------------------------------------
// Mesafasla
//--- Description -----------------------------------------------------------
// NPCs found in and around Mesafasla.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FTableland281NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_tableland_28_1", -3183.61, 266.24, 1135.78, 90, "TREASUREBOX_LV_F_TABLELAND_28_11000", "", "");
	}
}
