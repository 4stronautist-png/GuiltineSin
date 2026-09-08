//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Mission_test
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class mission_testWarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Mission_test to Royal Mausoleum 3F
		AddWarp(1, "ZACHARIEL35_1_ZACHARIEL34_1", 90, From("mission_test", -796, 99), To("d_zachariel_34", -2778, 151));
	}
}
