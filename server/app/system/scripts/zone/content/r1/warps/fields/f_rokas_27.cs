//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Akmens Ridge
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_rokas_27WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Akmens Ridge to Overlong Bridge Valley
		AddWarp("ROKAS27_ROKAS26", -2, From("f_rokas_27", -177, -3637), To("f_rokas_26", -1018, -1758));

		// Akmens Ridge to Tiltas Valley
		AddWarp("ROKAS27_ROKAS28", 239, From("f_rokas_27", -173, -1189), To("f_rokas_28", -637, -1704));
	}
}
