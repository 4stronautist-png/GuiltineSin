//--- GuiltineSin Script ----------------------------------------------------------
// Starving Demon's Way
//--- Description -----------------------------------------------------------
// NPCs found in and around Starving Demon's Way.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FPilgrimroad46NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(700, 147392, "Lv1 Treasure Chest", "f_pilgrimroad_46", -866.41, 220.58, -659.89, 225, "TREASUREBOX_LV_F_PILGRIMROAD_46700", "", "");
	}
}
