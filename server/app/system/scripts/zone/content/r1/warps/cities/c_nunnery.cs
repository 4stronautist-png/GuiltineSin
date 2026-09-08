//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Saalus Convent
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class c_nunneryWarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Saalus Convent to Pilgrim Path
		AddWarp("NUNNERY_PILGRIM47", 90, From("c_nunnery", 591, 146), To("f_pilgrimroad_47", -1124, 1360));
	}
}
