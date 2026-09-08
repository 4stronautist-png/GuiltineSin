//--- GuiltineSin Script ----------------------------------------------------------
// Grynas Training Camp
//--- Description -----------------------------------------------------------
// NPCs found in and around Grynas Training Camp.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FKatyn452NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_katyn_45_2", 339.44, 165.83, 1384.53, 135, "TREASUREBOX_LV_F_KATYN_45_21000", "", "");
	}
}
