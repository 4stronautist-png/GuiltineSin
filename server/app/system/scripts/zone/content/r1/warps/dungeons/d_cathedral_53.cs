//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Main Building
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_cathedral_53WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Main Building to Apsimesti Crossroads
		AddWarp("CATHEDRAL53_PILGRIM52", 360, From("d_cathedral_53", 17, -1133), To("f_pilgrimroad_52", 932, -779));

		// Main Building to Grand Corridor
		AddWarp("CATHEDRAL53_CATHEDRAL54", 94, From("d_cathedral_53", 1166.146, -285.1666), To("d_cathedral_54", 1107, -390));
	}
}
