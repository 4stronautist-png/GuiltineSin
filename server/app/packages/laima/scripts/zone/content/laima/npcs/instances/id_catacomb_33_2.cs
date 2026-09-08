//--- GuiltineSin Script ----------------------------------------------------------
// Carlyle's Mausoleum
//--- Description -----------------------------------------------------------
// NPCs found in and around Carlyle's Mausoleum.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class IdCatacomb332NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(259, 147392, "Lv1 Treasure Chest", "id_catacomb_33_2", 1449, 153, 223, 0, "TREASUREBOX_LV_ID_CATACOMB_33_2259", "", "");
	}
}
