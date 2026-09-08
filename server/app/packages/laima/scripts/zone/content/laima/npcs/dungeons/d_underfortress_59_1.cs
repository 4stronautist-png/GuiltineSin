//--- GuiltineSin Script ----------------------------------------------------------
// Royal Mausoleum Workers Lodge
//--- Description -----------------------------------------------------------
// NPCs found in and around Royal Mausoleum Workers Lodge.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DUnderfortress591NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(38, 147392, "Lv1 Treasure Chest", "d_underfortress_59_1", 59, 236, -1362, 90, "TREASUREBOX_LV_D_UNDERFORTRESS_59_138", "", "");
	}
}
