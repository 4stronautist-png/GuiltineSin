//--- GuiltineSin Script ----------------------------------------------------------
// Mullers Passage
//--- Description -----------------------------------------------------------
// NPCs found in and around Mullers Passage.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class PCatacomb1NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(4, 40120, "Statue of Goddess Vakarine", "p_catacomb_1", 63.74635, 260.2255, 8.646395, 30, "WARP_P_CATACOMB_1", "STOUP_CAMP", "STOUP_CAMP");
	}
}
