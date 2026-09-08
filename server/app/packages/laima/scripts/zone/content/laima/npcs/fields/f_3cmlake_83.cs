//--- GuiltineSin Script ----------------------------------------------------------
// Pelke Shrine Ruins
//--- Description -----------------------------------------------------------
// NPCs found in and around Pelke Shrine Ruins.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class F3cmlake83NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(172, 147392, "Lv1 Treasure Chest", "f_3cmlake_83", -664, 303, 819, 270, "TREASUREBOX_LV_F_3CMLAKE_83172", "", "");
	}
}
