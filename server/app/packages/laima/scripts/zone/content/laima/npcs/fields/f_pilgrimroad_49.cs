//--- GuiltineSin Script ----------------------------------------------------------
// Genar Field
//--- Description -----------------------------------------------------------
// NPCs found in and around Genar Field.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FPilgrimroad49NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_pilgrimroad_49", -169.54, 294.01, -1567.06, 225, "TREASUREBOX_LV_F_PILGRIMROAD_491000", "", "");
	}
}
