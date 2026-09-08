//--- GuiltineSin Script ----------------------------------------------------------
// Rasvoy Lake
//--- Description -----------------------------------------------------------
// NPCs found in and around Rasvoy Lake.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FPilgrimroad413NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(5, 40120, "Statue of Goddess Vakarine", "f_pilgrimroad_41_3", -899.8269, 62.01554, 515.5572, 45, "WARP_PILGRIMROAD_41_3", "STOUP_CAMP", "STOUP_CAMP");
		
	}
}
