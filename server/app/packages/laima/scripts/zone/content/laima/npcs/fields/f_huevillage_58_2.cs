//--- GuiltineSin Script ----------------------------------------------------------
// Vieta Gorge
//--- Description -----------------------------------------------------------
// NPCs found in and around Vieta Gorge.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FHuevillage582NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(34, 40120, "Statue of Goddess Vakarine", "f_huevillage_58_2", -515.8, 271.89, -1541.66, 125, "WARP_F_HUEVILLAGE_58_2", "STOUP_CAMP", "STOUP_CAMP");

		// Papaya main quest actors (hidenpc.ies: 571, 572, 573, 574).
		//-------------------------------------------------------------------------
		AddNpc(571, 147396, "Andale Village Elder", "f_huevillage_58_2", -605, 274, -1185, 90, "HUEVILLAGE_58_2_MQ01_NPC", "", "");
		AddNpc(572, 153174, "Andale Village Priest", "f_huevillage_58_2", 245, 116, 230, 180, "HUEVILLAGE_58_2_MQ02_NPC", "", "");
		AddNpc(573, 147501, "Broken Obelisk", "f_huevillage_58_2", -107, 41, 1189, 0, "HUEVILLAGE_58_2_OBELISK_BEFORE", "", "", (int)NpcState.Normal, 260);

		// White Oak sap collection containers for Activate the Obelisk (1).
		//-------------------------------------------------------------------------
		AddNpc(575, 147354, "Tree Sap Collection Container", "f_huevillage_58_2", -210, 41, 1115, 0, "HUEVILLAGE_58_2_MQ02_BUCKET01", "", "", (int)NpcState.Normal, 260);
		AddNpc(576, 147354, "Tree Sap Collection Container", "f_huevillage_58_2", -85, 41, 1215, 0, "HUEVILLAGE_58_2_MQ02_BUCKET02", "", "", (int)NpcState.Normal, 260);
		AddNpc(577, 147354, "Tree Sap Collection Container", "f_huevillage_58_2", 30, 41, 1080, 0, "HUEVILLAGE_58_2_MQ02_BUCKET03", "", "", (int)NpcState.Normal, 260);

		// Ershike altar used by Activate the Obelisk (2).
		//-------------------------------------------------------------------------
		AddNpc(578, 147414, "Ershike Altar", "f_huevillage_58_2", -365, 43, 690, 0, "HUEVILLAGE_58_2_MQ03_NPC", "", "", (int)NpcState.Normal, 260);

		// Lv1 Treasure Chest
		//-------------------------------------------------------------------------
		AddNpc(49, 147392, "Lv1 Treasure Chest", "f_huevillage_58_2", -159.95, 274.31, -1274.28, 90, "TREASUREBOX_LV_F_HUEVILLAGE_58_249", "", "");
	}
}
