//--- GuiltineSin Script ----------------------------------------------------------
// Srautas Gorge
//--- Description -----------------------------------------------------------
// NPCs found in and around Srautas Gorge.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FGele571NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153006, "", "f_gele_57_1", -305.9306, 168.8233, -402.949, 0, "f_gele_57_1_elt", 2, 1);
	}
}
