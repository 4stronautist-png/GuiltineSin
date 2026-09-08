//--- GuiltineSin Script ----------------------------------------------------------
// Syla Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Syla Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FWhitetrees233NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_whitetrees_23_3", 388.16, 219.3, 1280.83, 90, "TREASUREBOX_LV_F_WHITETREES_23_31000", "", "");
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157037, "", "f_whitetrees_23_3", -997.1872, 160.0814, 367.3382, 38, "f_whitetrees_23_3_elt", 2, 1);

	}
}
