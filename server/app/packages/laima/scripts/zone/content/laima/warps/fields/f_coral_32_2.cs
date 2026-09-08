//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Igti Coast
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_coral_32_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Igti Coast to Cranto Coast
		AddWarp(1, "CORAL32_2_CORAL32_1", 16, From("f_coral_32_2", -33.89635, -1051.392), To("f_coral_32_1", -286, 841));
	}
}
