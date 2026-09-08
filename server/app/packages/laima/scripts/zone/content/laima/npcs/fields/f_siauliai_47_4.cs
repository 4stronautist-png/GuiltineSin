//--- GuiltineSin Script ----------------------------------------------------------
// Baron Allerno
//--- Description -----------------------------------------------------------
// NPCs found in and around Baron Allerno.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FSiauliai474NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(34, 147392, "Lv1 Treasure Chest", "f_siauliai_47_4", 2146.31, 210.09, -767.64, 90, "TREASUREBOX_LV_F_SIAULIAI_47_434", "", "");
		
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(36, 147392, "Lv1 Treasure Chest", "f_siauliai_47_4", 1152.09, 43.87, 170.53, 90, "TREASUREBOX_LV_F_SIAULIAI_47_436", "", "");
	}
}
