//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Tenet Church 2F
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_chapel_57_7WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Tenet Church 2F to Tenet Church 1F
		AddWarp(1, "CHAPEL577_CHAPEL576", 180, From("d_chapel_57_7", -683, -831), To("d_chapel_57_6", -1487, 299));
	}
}
