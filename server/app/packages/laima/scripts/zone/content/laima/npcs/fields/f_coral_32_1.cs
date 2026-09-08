//--- GuiltineSin Script ----------------------------------------------------------
// Cranto Coast
//--- Description -----------------------------------------------------------
// NPCs found in and around Cranto Coast.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCoral321NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_coral_32_1", 376.18, 236.88, -1042.87, 225, "TREASUREBOX_LV_F_CORAL_32_11000", "", "");
	}
}
