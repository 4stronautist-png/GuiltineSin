//--- GuiltineSin Script ----------------------------------------------------------
// Ashaq Underground Prison 3F
//--- Description -----------------------------------------------------------
// NPCs found in and around Ashaq Underground Prison 3F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DPrison623NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(154058, "", "d_prison_62_3", -324.5829, 1013.862, 329.7611, 0, "d_prison_62_3_elt", 2, 1);
	}
}
