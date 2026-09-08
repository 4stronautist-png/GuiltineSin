//--- GuiltineSin Script ----------------------------------------------------------
// Barha Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Barha Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FOrchard343NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_orchard_34_3", -855.38, 370.68, 410, 90, "TREASUREBOX_LV_F_ORCHARD_34_31000", "", "");
	}
}
