//--- GuiltineSin Script ----------------------------------------------------------
// Goddess' Ancient Garden
//--- Description -----------------------------------------------------------
// NPCs found in and around Goddess' Ancient Garden.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FRemains38NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(74, 40120, "Statue of Goddess Vakarine", "f_remains_38", 340.66, 277.95, -457.09, 0, "WARP_F_REMAINS_38", "STOUP_CAMP", "STOUP_CAMP");
		
		// Lv3 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(86, 147393, "Lv3 Treasure Chest", "f_remains_38", 973.32, 244.22, -1043.33, 0, "TREASUREBOX_LV_F_REMAINS_3886", "", "");
		
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(88, 147392, "Lv1 Treasure Chest", "f_remains_38", 1725.47, 370.31, 1206.89, 90, "TREASUREBOX_LV_F_REMAINS_3888", "", "");
	}
}
