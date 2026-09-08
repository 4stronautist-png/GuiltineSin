//--- GuiltineSin Script ----------------------------------------------------------
// Tiltas Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Tiltas Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FMaple252NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157035, "", "f_maple_25_2", -78.02134, 34.58647, 382.6509, 320, "f_maple_25_2_elt", 2, 1);
	}
}
