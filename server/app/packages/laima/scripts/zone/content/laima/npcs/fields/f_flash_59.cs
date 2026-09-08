//--- GuiltineSin Script ----------------------------------------------------------
// Verkti Square
//--- Description -----------------------------------------------------------
// NPCs found in and around Verkti Square.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FFlash59NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_flash_59", 818.07, 61.83, 798.19, 90, "TREASUREBOX_LV_F_FLASH_591000", "", "");
	}
}
