//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Storage Quarter
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_underfortress_68WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Storage Quarter to Resident Quarter
		AddWarp(3, "UNDERFORTRESS68_UNDERFORTRESS67", 262, From("d_underfortress_68", -2897.62, -1228.351), To("d_underfortress_67", 356, 400));

		// Storage Quarter to Fortress Battlegrounds
		AddWarp(4, "UNDERFORTRESS68_UNDERFORTRESS69", 200, From("d_underfortress_68", 1746.333, 1810.33), To("d_underfortress_69", 2189, 9));
	}
}
