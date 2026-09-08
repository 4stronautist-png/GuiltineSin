//--- GuiltineSin Script ----------------------------------------------------------
// 2nd Demon Prison
//--- Description -----------------------------------------------------------
// NPCs found in and around 2nd Demon Prison.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DVelniasprison541NpcScript : GeneralScript
{
	protected override void Load()
	{

		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(52, 147392, "Lv1 Treasure Chest", "d_velniasprison_54_1", 507, 90, -608, 0, "TREASUREBOX_LV_D_VELNIASPRISON_54_152", "", "");
	}
}
