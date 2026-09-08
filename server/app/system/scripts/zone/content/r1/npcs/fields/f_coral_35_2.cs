//--- GuiltineSin Script ----------------------------------------------------------
// Vera Coast
//--- Description -----------------------------------------------------------
// NPCs found in and around Vera Coast.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCoral352NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(156034, "", "f_coral_35_2", 401.4194, 224.4277, 717.4001, 0, "f_coral_35_2_elt", 2, 1);
	}
}
