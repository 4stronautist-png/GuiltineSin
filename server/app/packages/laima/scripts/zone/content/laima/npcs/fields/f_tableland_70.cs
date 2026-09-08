//--- GuiltineSin Script ----------------------------------------------------------
// Ibre Plateau
//--- Description -----------------------------------------------------------
// NPCs found in and around Ibre Plateau.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FTableland70NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_tableland_70", 4624.72, 639.57, -2557.13, -45, "TREASUREBOX_LV_F_TABLELAND_701000", "", "");
	}
}
