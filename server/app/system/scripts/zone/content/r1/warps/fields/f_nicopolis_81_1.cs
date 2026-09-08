//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Starry Town
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class f_nicopolis_81_1WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Starry Town to Istora Ruins
		AddWarp("NICO811_REMAINS37_3", -18, From("f_nicopolis_81_1", 1647.505, -1414.545), To("f_remains_37_3", -702, 681));

		// Starry Town to Feline Post Town
		AddWarp("NICO811_NICO812", 174, From("f_nicopolis_81_1", -1602.811, 788.4332), To("f_nicopolis_81_2", 1672, -1924));
	}
}
