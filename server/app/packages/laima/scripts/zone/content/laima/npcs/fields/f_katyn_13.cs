//--- GuiltineSin Script ----------------------------------------------------------
// Poslinkis Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Poslinkis Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FKatyn13NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(773, 147392, "Lv1 Treasure Chest", "f_katyn_13", -1017.59, 237.9, -1066.07, 90, "TREASUREBOX_LV_F_KATYN_13773", "", "");
	}
}
