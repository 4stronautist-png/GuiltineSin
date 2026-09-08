//--- GuiltineSin Script ----------------------------------------------------------
// Main Building
//--- Description -----------------------------------------------------------
// NPCs found in and around Main Building.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DCathedral53NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(63, 147392, "Lv1 Treasure Chest", "d_cathedral_53", 228.81, 0.1, -313.9, 180, "TREASUREBOX_LV_D_CATHEDRAL_5363", "", "");
	}
}
