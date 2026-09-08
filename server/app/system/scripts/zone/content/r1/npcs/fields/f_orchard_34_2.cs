//--- GuiltineSin Script ----------------------------------------------------------
// Ruklys Hall
//--- Description -----------------------------------------------------------
// NPCs found in and around Ruklys Hall.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FOrchard342NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(155059, "", "f_orchard_34_2", -1761.933, -357.2179, 384.1256, 90, "f_orchard_34_2_elevator", 3, 2);
	}
}
