//--- GuiltineSin Script ----------------------------------------------------------
// Gele Plateau
//--- Description -----------------------------------------------------------
// NPCs found in and around Gele Plateau.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FGele572NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153007, "", "f_gele_57_2", -85.66, 381.08, 841.97, 0, "f_gele57_2_cablecar", 2, 5);
	}
}
