//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Bokor Master's Home
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class c_voodooWarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Bokor Master's Home to Klaipeda
		AddWarp("WS_BOCORS_KLAPEDA", 20, From("c_voodoo", 43, -129), To("c_Klaipe", -939, -527));
	}
}
