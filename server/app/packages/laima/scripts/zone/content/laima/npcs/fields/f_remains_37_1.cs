//--- GuiltineSin Script ----------------------------------------------------------
// Nuoridin Falls
//--- Description -----------------------------------------------------------
// NPCs found in and around Nuoridin Falls.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FRemains371NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_remains_37_1", -835.83, 445.11, 1575.17, 0, "TREASUREBOX_LV_F_REMAINS_37_11000", "", "");
	}
}
