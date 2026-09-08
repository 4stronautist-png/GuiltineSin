//--- GuiltineSin Script ----------------------------------------------------------
// Royal Mausoleum 3F
//--- Description -----------------------------------------------------------
// NPCs found in and around Royal Mausoleum 3F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DZachariel34NpcScript : GeneralScript
{
	protected override void Load()
	{

		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(3027, 147392, "Lv1 Treasure Chest", "d_zachariel_34", -3221.72, 296.99, -22.04, 0, "TREASUREBOX_LV_D_ZACHARIEL_343027", "", "");
	}
}
