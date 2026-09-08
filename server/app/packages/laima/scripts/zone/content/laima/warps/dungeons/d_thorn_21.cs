//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Kvailas Forest
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_thorn_21WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Kvailas Forest to Gate Route
		AddWarp(122, "THORN21_THORN19", 270, From("d_thorn_21", -413, -40), To("d_thorn_19", 2061, -936));
	}
}
