//--- GuiltineSin Script ----------------------------------------------------------
// Spell Tome Town
//--- Description -----------------------------------------------------------
// NPCs found in and around Spell Tome Town.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FNicopolis813NpcScript : GeneralScript
{
	protected override void Load()
	{
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153233, "", "f_nicopolis_81_3", -372.14, -157.52, -888.32, 1, "f_nicopolis_81_3_elt", 4, 1);

	}
}
