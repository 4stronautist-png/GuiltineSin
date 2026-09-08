//--- GuiltineSin Script ----------------------------------------------------------
// Lhadar Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Lhadar Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FMaple253NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_maple_25_3", -376.79, 380.54, 480.66, 90, "TREASUREBOX_LV_F_MAPLE_25_31000", "", "");
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157036, "", "f_maple_25_3", -917.34, 36.80652, -444.9715, 0, "f_maple_25_3_elt", 2, 1);

	}
}
