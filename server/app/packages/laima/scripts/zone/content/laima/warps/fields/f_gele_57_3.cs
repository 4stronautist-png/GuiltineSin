//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Nefritas Cliff
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_gele_57_3WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Nefritas Cliff to Gele Plateau
		AddWarp(1, "GELE573_TO_GELE572", 5, From("f_gele_57_3", -509.8248, -1593.792), To("f_gele_57_2", 72, 1456));

		// Nefritas Cliff to Vieta Gorge
		AddWarp(3, "GELE573_TO_HUEVILLAGE_58_2", 315, From("f_gele_57_3", -1400, -809), To("f_huevillage_58_2", 1457, 1086));

		// Nefritas Cliff to the Hidden Sanctum route
		AddWarp(4, "GELE573_TO_HUE581", 315, From("f_gele_57_3", -1307, -684), To("f_huevillage_58_1", 1562, -1188));

		// Nefritas Cliff to Tenet Garden
		AddWarp(2, "GELE573_TO_GELE574", 180, From("f_gele_57_3", 199.4143, 1157.455), To("f_gele_57_4", -833, -48));

		// Nefritas Cliff to Genar Field
		AddWarp(38, "GELE57_3_PILGRIM49", 90, From("f_gele_57_3", 1494.618, -711.321), To("f_pilgrimroad_49", -2142, -571));
	}
}
