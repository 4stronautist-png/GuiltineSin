//--- GuiltineSin Script ----------------------------------------------------------
// Arcus Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Arcus Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FBracken431NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "f_bracken_43_1", -401.92, 188.43, -501.73, 90, "TREASUREBOX_LV_F_BRACKEN_43_11000", "", "");
	}
}
