//--- GuiltineSin Script ----------------------------------------------------------
// Altar Way
//--- Description -----------------------------------------------------------
// NPCs found in and around Altar Way.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FPilgrimroad50NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(41, 147392, "Lv1 Treasure Chest", "f_pilgrimroad_50", -969.98, 557.98, 1142.73, 90, "TREASUREBOX_LV_F_PILGRIMROAD_5041", "", "");
	}
}
