//--- GuiltineSin Script ----------------------------------------------------------
// Mage Tower 4F
//--- Description -----------------------------------------------------------
// NPCs found in and around Mage Tower 4F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DFiretower44NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(230, 147392, "Lv1 Treasure Chest", "d_firetower_44", 744.7, 451.3, 554.75, 90, "TREASUREBOX_LV_D_FIRETOWER_44230", "", "");
	}
}
