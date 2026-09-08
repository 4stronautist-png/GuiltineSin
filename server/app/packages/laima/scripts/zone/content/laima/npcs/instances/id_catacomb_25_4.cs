//--- GuiltineSin Script ----------------------------------------------------------
// Timerys Temple
//--- Description -----------------------------------------------------------
// NPCs found in and around Timerys Temple.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class IdCatacomb254NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "id_catacomb_25_4", -944.7, -30.33, 258.42, 90, "TREASUREBOX_LV_ID_CATACOMB_25_41000", "", "");
	}
}
