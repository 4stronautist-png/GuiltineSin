//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Spell Tome Town
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_nicopolis_81_3WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Spell Tome Town to Feline Post Town
		AddWarp(8, "NICO813_NICO812", 90, From("f_nicopolis_81_3", 2914.409, 1168.289), To("f_nicopolis_81_2", -2405, -993));
	}
}
