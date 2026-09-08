//--- GuiltineSin Script ----------------------------------------------------------
// Sacred Atspalvis 
//--- Description -----------------------------------------------------------
// NPCs found in and around Sacred Atspalvis .
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle101NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(2, 40120, "Statue of Goddess Vakarine", "f_castle_101", 92.4603, 240.7966, -1390.964, 5, "WARP_F_CASTLE_101", "STOUP_CAMP", "STOUP_CAMP");
	}
}
