//--- GuiltineSin Script ----------------------------------------------------------
// Nevellet Quarry 1F
//--- Description -----------------------------------------------------------
// NPCs found in and around Nevellet Quarry 1F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DCmine661NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(155110, "", "d_cmine_66_1", 77.5127, 413.2733, -77.08086, 0, "d_cmine_66_1_elt", 2, 1);
	}
}
