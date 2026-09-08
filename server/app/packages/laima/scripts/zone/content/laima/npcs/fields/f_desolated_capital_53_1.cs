//--- GuiltineSin Script ----------------------------------------------------------
// Rinksmas Ruins
//--- Description -----------------------------------------------------------
// NPCs found in and around Rinksmas Ruins.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FDesolatedCapital531NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(9, 40120, "Statue of Goddess Vakarine", "f_desolated_capital_53_1", 1515.549, 111.8744, 2274.188, 45, "WARP_DCAPITAL53_1", "STOUP_CAMP", "STOUP_CAMP");
	}
}
