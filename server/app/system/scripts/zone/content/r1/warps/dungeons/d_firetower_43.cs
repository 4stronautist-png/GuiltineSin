//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Mage Tower 3F
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_firetower_43WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Mage Tower 3F to Mage Tower 2F
		AddWarp("FIRETOWER43_TO_FIRETOWER42", 65, From("d_firetower_43", -2473, -294), To("d_firetower_42", -1010, -2410));
	}
}
