//--- GuiltineSin Script ----------------------------------------------------------
// Tevhrin Stalactite Cave Section 3
//--- Description -----------------------------------------------------------
// NPCs found in and around Tevhrin Stalactite Cave Section 3.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DLimestonecave523NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "d_limestonecave_52_3", -1052.52, 0.1, 276.4, 90, "TREASUREBOX_LV_D_LIMESTONECAVE_52_31000", "", "");
	}
}
