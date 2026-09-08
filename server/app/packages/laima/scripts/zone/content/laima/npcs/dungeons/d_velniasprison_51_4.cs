//--- GuiltineSin Script ----------------------------------------------------------
// Demon Prison District 3
//--- Description -----------------------------------------------------------
// NPCs found in and around Demon Prison District 3.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DVelniasprison514NpcScript : GeneralScript
{
	protected override void Load()
	{

		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(32, 147392, "Lv1 Treasure Chest", "d_velniasprison_51_4", -1883.23, 228.83, -283.63, 0, "TREASUREBOX_LV_D_VELNIASPRISON_51_432", "", "");
	}
}
