//--- GuiltineSin Script ----------------------------------------------------------
// Central Parias Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Central Parias Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FMaple241NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(8, 40120, "Statue of Goddess Vakarine", "f_maple_24_1", 1440.543, 1.0349, 1481.02, 25, "WARP_F_MAPLE_24_1", "STOUP_CAMP", "STOUP_CAMP");
	}
}
