//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Goddess' Ancient Garden
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_remains_38WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Goddess' Ancient Garden to Stele Road
		AddWarp("REMAINS38_REMAINS37", 0, From("f_remains_38", -1400, -2214), To("f_remains_37", 563, 3270));

		// Goddess' Ancient Garden to Escanciu Village
		AddWarp("REMAINS38_REMAINS39", 225, From("f_remains_38", 1358, 1740), To("f_remains_39", -524, -590));
	}
}
