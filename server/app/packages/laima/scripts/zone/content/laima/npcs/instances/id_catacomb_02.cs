//--- GuiltineSin Script ----------------------------------------------------------
// Valius' Eternal Resting Place
//--- Description -----------------------------------------------------------
// NPCs found in and around Valius' Eternal Resting Place.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class IdCatacomb02NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(1000, 147392, "Lv1 Treasure Chest", "id_catacomb_02", -1200.89, 289.92, 1646.15, 225, "TREASUREBOX_LV_ID_CATACOMB_021000", "", "");
	}
}
