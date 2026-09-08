//--- GuiltineSin Script ----------------------------------------------------------
// Spring Light Woods
//--- Description -----------------------------------------------------------
// NPCs found in and around Spring Light Woods.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FSiauliai461NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(27, 147392, "Lv1 Treasure Chest", "f_siauliai_46_1", 954, 390.89, 1020.98, 90, "TREASUREBOX_LV_F_SIAULIAI_46_127", "", "");
	}
}
