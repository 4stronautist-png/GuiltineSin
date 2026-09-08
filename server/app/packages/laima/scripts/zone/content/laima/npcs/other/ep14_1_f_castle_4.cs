//--- GuiltineSin Script ----------------------------------------------------------
// Delmore Citadel
//--- Description -----------------------------------------------------------
// NPCs found in and around Delmore Citadel.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class Ep141FCastle4NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(84, 40120, "Statue of Goddess Vakarine", "ep14_1_f_castle_4", 1442.492, 58.91404, -2066.39, 220, "WARP_EP14_1_F_CASTLE_4", "STOUP_CAMP", "STOUP_CAMP");
	}
}
