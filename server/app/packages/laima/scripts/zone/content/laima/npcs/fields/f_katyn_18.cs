//--- GuiltineSin Script ----------------------------------------------------------
// Kule Peak
//--- Description -----------------------------------------------------------
// NPCs found in and around Kule Peak.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FKatyn18NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_katyn_18", 1898.66, 373.29, -1206.73, 90, "TREASUREBOX_LV_F_KATYN_181000", "", "");
	}
}
