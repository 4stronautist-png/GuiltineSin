//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Owl Burial Ground
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_katyn_7_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Owl Burial Ground to Poslinkis Forest
		AddWarp(1, "KATYN7_2_KATYN13", 270, From("f_katyn_7_2", 273, 1750), To("f_katyn_13", -41, -2127));
	}
}
