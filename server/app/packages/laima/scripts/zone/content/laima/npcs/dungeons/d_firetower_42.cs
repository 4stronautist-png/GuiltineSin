//--- GuiltineSin Script ----------------------------------------------------------
// Mage Tower 2F
//--- Description -----------------------------------------------------------
// NPCs found in and around Mage Tower 2F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DFiretower42NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(223, 147392, "Lv1 Treasure Chest", "d_firetower_42", 2066.69, 20.58, -593.81, 90, "TREASUREBOX_LV_D_FIRETOWER_42223", "", "");
	}
}
