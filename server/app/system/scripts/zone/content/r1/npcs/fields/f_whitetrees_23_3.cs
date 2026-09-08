//--- GuiltineSin Script ----------------------------------------------------------
// Syla Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Syla Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FWhitetrees233NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157037, "", "f_whitetrees_23_3", -997.1872, 160.0814, 367.3382, 38, "f_whitetrees_23_3_elt", 2, 1);
	}
}
