//--- GuiltineSin Script ----------------------------------------------------------
// Delmore Outskirts
//--- Description -----------------------------------------------------------
// NPCs found in and around Delmore Outskirts.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle653NpcScript : GeneralScript
{
	protected override void Load()
	{
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(155109, "", "f_castle_65_3", 123.7928, 96.6724, 200.1542, 0, "f_castle_65_3_elt", 2, 1);


		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(441, 147392, "Lv1 Treasure Chest", "f_castle_65_3", -1597.13, 45.32, 86.78, -135, "TREASUREBOX_LV_F_CASTLE_65_3441", "", "");
	}
}
