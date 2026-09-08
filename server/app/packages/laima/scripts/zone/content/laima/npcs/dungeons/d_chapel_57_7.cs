//--- GuiltineSin Script ----------------------------------------------------------
// Tenet Church 2F
//--- Description -----------------------------------------------------------
// NPCs found in and around Tenet Church 2F.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DChapel577NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(60, 147392, "Lv1 Treasure Chest", "d_chapel_57_7", -738.51, 36.02, 125.63, 90, "TREASUREBOX_LV_D_CHAPEL_57_760", "", "");

		// Papaya main quest actors: Hidden Sanctum
		//-------------------------------------------------------------------------
		AddNpc(70, 147390, "Paladin Follower", "d_chapel_57_7", -683, 35.917, -938, 0, "CHAPLE577_ARUNE_01", "", "");
		AddNpc(71, 147390, "Paladin Follower", "d_chapel_57_7", 95, 164, -731, 0, "CHAPLE577_ARUNE_02", "", "");
		AddNpc(72, 12080, "Holy Altar", "d_chapel_57_7", 95, 164, -731, 0, "CHAPLE577_HOLY_1", "", "");
		AddNpc(73, 12080, "Holy Altar", "d_chapel_57_7", -72, 35, 625, 0, "CHAPLE577_HOLY_2", "", "");
		AddNpc(74, 12080, "Holy Altar", "d_chapel_57_7", -30, 35, -127, 0, "CHAPLE577_HOLY_3", "", "");
		AddNpc(75, 152003, "Secret Warp Portal", "d_chapel_57_7", -30, 35, -127, 0, "CHAPLE577_MQ_10", "", "");

		AddNpc(81, 12080, "Central Pillar", "d_chapel_57_7", -92, 35, 390, 0, "CHAPLE577_MQ_04_1", "", "");
		AddNpc(82, 12080, "Central Pillar", "d_chapel_57_7", 18, 35, 649, 0, "CHAPLE577_MQ_04_2", "", "");
		AddNpc(83, 12080, "Central Pillar", "d_chapel_57_7", 99, 35, 1181, 0, "CHAPLE577_MQ_04_3", "", "");
		AddNpc(84, 12080, "Central Pillar", "d_chapel_57_7", -121, 35, 1352, 0, "CHAPLE577_MQ_04_4", "", "");
		AddNpc(85, 12080, "Central Pillar", "d_chapel_57_7", -807, 36, 36, 0, "CHAPLE577_MQ_04_5", "", "");
		AddNpc(86, 12080, "Central Pillar", "d_chapel_57_7", -425, 35, -113, 0, "CHAPLE577_MQ_04_6", "", "");
		AddNpc(87, 12080, "Central Pillar", "d_chapel_57_7", 461, 35, -122, 0, "CHAPLE577_MQ_04_7", "", "");
		AddNpc(88, 12080, "Central Pillar", "d_chapel_57_7", 831, 35, -190, 0, "CHAPLE577_MQ_04_8", "", "");
	}
}
