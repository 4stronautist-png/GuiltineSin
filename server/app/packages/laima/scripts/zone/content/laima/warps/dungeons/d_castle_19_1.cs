//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Sajunga Road
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_castle_19_1WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Sajunga Road to Frienel Memorial
		AddWarp(7, "D_CASTLE_19_1_TO_F_DCAPITAL_107", 179, From("d_castle_19_1", 2.432629, 1008.354), To("f_dcapital_107", 3, 1688));

		// Sajunga Road to Vienibe Shelter
		AddWarp(8, "D_CASTLE_19_1_TO_F_CASTLE_97", 180, From("d_castle_19_1", -4.410236, 373.3464), To("f_castle_97", -94, 1831));
	}
}
