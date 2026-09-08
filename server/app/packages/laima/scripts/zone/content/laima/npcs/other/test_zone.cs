//--- GuiltineSin Script ----------------------------------------------------------
// Test Zone
//--- Description -----------------------------------------------------------
// NPCs found in and around Test Zone.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class TestZoneNpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Ausrine
		//-------------------------------------------------------------------------
		AddNpc(108, 40130, "Statue of Goddess Ausrine", "test_zone", 38, 0, 190, 90, "SKILLPOINTUP2", "", "");
	}
}
