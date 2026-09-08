//--- GuiltineSin Script ----------------------------------------------------------
// Epherotao Coast
//--- Description -----------------------------------------------------------
// NPCs found in and around Epherotao Coast.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCoral443NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157050, "", "f_coral_44_3", 464.66, 92.97, 518.12, 335, "f_coral_44_3_elt", 2, 1);
	}
}
