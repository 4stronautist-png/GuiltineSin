//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Mage Tower 2F
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_firetower_42WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Mage Tower 2F to Mage Tower 1F
		AddWarpPortal(From("d_firetower_42", 2522, -560), To("d_firetower_41", 2849, -1412));

		// Mage Tower 2F to Mage Tower 3F
		AddWarp(9, "FIRETOWER42_TO_FIRETOWER43", 95, From("d_firetower_42", -933, -2391), To("d_firetower_43", -2508, -258));
	}
}
