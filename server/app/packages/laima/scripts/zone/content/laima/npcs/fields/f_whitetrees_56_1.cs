//--- GuiltineSin Script ----------------------------------------------------------
// Mishekan Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Mishekan Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FWhitetrees561NpcScript : GeneralScript
{
	protected override void Load()
	{

		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157038, "", "f_whitetrees_56_1", 1384.889, 68.06953, 243.3077, 10, "f_whitetrees_56_1_elt", 2, 1);

	}
}
