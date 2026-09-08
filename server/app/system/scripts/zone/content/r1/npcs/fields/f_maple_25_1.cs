//--- GuiltineSin Script ----------------------------------------------------------
// Nheto Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Nheto Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FMaple251NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157034, "", "f_maple_25_1", -953.6499, -38.68602, 177.3624, 342, "f_maple_25_1_elt", 2, 1);
	}
}
