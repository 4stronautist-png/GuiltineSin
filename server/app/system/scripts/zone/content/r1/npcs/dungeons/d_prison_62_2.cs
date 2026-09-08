//--- GuiltineSin Script ----------------------------------------------------------
// Ashaq Underground Prison 2F
//--- Description -----------------------------------------------------------
// NPCs found in and around Ashaq Underground Prison 2F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DPrison622NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(154058, "", "d_prison_62_2", 5.702232, -707.8696, -1245.999, 0, "d_prison_62_2_elt", 2, 1);
	}
}
