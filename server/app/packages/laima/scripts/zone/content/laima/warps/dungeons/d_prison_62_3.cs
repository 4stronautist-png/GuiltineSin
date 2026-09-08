//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Ashaq Underground Prison 3F
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_prison_62_3WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Ashaq Underground Prison 3F to Ashaq Underground Prison 2F
		AddWarp(4, "PRISON623_TO_PRISON622", 0, From("d_prison_62_3", 890.1995, -707.2404), To("d_prison_62_2", 176, -1266));
	}
}
