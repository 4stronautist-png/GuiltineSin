//--- GuiltineSin Script ----------------------------------------------------------
// Feline Post Town
//--- Description -----------------------------------------------------------
// NPCs found in and around Feline Post Town.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FNicopolis812NpcScript : GeneralScript
{
	protected override void Load()
	{
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153232, "", "f_nicopolis_81_2", 60.4, 2.22, -1645, 0, "f_nicopolis_81_2_elt", 2, 1);

	}
}
