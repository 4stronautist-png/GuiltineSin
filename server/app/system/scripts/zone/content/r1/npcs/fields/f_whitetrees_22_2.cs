//--- GuiltineSin Script ----------------------------------------------------------
// Tekel Shelter
//--- Description -----------------------------------------------------------
// NPCs found in and around Tekel Shelter.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FWhitetrees222NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153189, "", "f_whitetrees_22_2", 990.0172, 125.708, -499.8661, 351, "f_whitetrees_22_2_elt", 2, 1);
	}
}
