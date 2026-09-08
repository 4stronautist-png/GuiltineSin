//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Ruklys Hall of Fame
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_underfortress_30_1WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Ruklys Hall of Fame to Resident Quarter
		AddWarp("UNDERFORTRESS30_1_UNDERFORTRESS67", 171, From("d_underfortress_30_1", -105.1161, 1942.654), To("d_underfortress_67", 920, 86));

		// Ruklys Hall of Fame to Extension
		AddWarp("UNDERFORTRESS30_1_UNDERFORTRESS30_2", 94, From("d_underfortress_30_1", 1030.448, -954.5347), To("d_underfortress_30_2", 0, 0));
	}
}
