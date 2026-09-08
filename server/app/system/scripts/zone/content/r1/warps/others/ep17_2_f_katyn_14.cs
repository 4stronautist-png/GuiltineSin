//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in EP17 Saknis Plains
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class ep17_2_f_katyn_14WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// EP17 Saknis Plains to EP17 Letas Stream
		AddWarp("EP17_2_KATYN_14_KATYN_12", 93, From("ep17_2_f_katyn_14", 3516, -887), To("ep17_2_f_katyn_12", 2141, 1040));
	}
}
