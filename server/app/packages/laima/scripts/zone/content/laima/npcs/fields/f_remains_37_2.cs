//--- GuiltineSin Script ----------------------------------------------------------
// Namu Temple Ruins
//--- Description -----------------------------------------------------------
// NPCs found in and around Namu Temple Ruins.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FRemains372NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_remains_37_2", -426.58, 2.6, 394.91, -45, "TREASUREBOX_LV_F_REMAINS_37_21000", "", "");
	}
}
