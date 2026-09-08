//--- GuiltineSin Script ----------------------------------------------------------
// Grand Yard Mesa
//--- Description -----------------------------------------------------------
// NPCs found in and around Grand Yard Mesa.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FTableland71NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_tableland_71", 186.31, 443.54, -125.44, -45, "TREASUREBOX_LV_F_TABLELAND_711000", "", "");
	}
}
