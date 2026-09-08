//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Vera Coast
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_coral_35_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Vera Coast to Nahash Forest
		AddWarp(4, "CORAL35_2_SIAULIAI_35_1", 260, From("f_coral_35_2", -1460.112, 1176.225), To("f_siauliai_35_1", 1449, 399));
	}
}
