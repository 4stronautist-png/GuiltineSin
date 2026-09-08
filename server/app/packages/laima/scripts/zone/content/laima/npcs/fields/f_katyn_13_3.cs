//--- GuiltineSin Script ----------------------------------------------------------
// Arrow Path
//--- Description -----------------------------------------------------------
// NPCs found in and around Arrow Path.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FKatyn133NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(10025, 147392, "Lv1 Treasure Chest", "f_katyn_13_3", -262, 199, -707, 45, "TREASUREBOX_LV_F_KATYN_710025", "", "");
	}
}
