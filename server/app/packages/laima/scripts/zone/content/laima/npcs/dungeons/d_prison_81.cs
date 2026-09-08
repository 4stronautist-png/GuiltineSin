//--- GuiltineSin Script ----------------------------------------------------------
// Workshop
//--- Description -----------------------------------------------------------
// NPCs found in and around Workshop.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DPrison81NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(6, 40120, "Statue of Goddess Vakarine", "d_prison_81", 817.2656, 192.2627, -725.8778, 90, "WARP_D_PRISON_81", "STOUP_CAMP", "STOUP_CAMP");
	}
}
