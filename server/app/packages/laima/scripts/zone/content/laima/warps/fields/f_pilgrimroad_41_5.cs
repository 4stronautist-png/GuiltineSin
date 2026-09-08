//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Ouaas Memorial
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_pilgrimroad_41_5WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Ouaas Memorial to Rasvoy Lake
		AddWarp(1, "PILGRIM41_5_PILGRIM41_3", -89, From("f_pilgrimroad_41_5", -1941.114, 576.4455), To("f_pilgrimroad_41_3", -1064, -1280));
	}
}
