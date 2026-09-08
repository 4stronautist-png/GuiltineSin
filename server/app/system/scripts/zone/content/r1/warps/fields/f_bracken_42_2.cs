//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Khamadon Forest
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_bracken_42_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Khamadon Forest to Khonot Forest
		AddWarp("BRACKEN42_2_TO_BRACKEN42_1", 257, From("f_bracken_42_2", -1735.163, 231.7298), To("f_bracken_42_1", 2024, -214));
	}
}
