//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Timerys Temple
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class id_catacomb_25_4WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Timerys Temple to Lhadar Forest
		AddWarp("CATACOMB_25_4_TO_MAPLE_25_3", 92, From("id_catacomb_25_4", 1664.911, 580.139), To("f_maple_25_3", -1841, 551));
	}
}
