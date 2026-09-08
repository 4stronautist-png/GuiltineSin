//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Southern Parias Forest
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_maple_24_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Southern Parias Forest to Central Parias Forest
		AddWarp("F_MAPLE_242_TO_F_MAPLE_241", 225, From("f_maple_24_2", 1376.938, 1265.504), To("f_maple_24_1", 1726, -155));
	}
}
