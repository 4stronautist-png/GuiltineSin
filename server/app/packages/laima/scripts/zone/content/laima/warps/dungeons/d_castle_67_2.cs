//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Topes Fortress 2F
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_castle_67_2WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Topes Fortress 2F to Topes Fortress 1F
		AddWarp(1, "CASTLE_67_2_TO_CASTLE_67_1", -87, From("d_castle_67_2", -1884.743, -1296.475), To("d_castle_67_1", 720, 1141));
	}
}
