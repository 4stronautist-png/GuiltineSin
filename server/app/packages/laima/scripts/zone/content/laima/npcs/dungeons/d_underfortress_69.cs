//--- GuiltineSin Script ----------------------------------------------------------
// Fortress Battlegrounds
//--- Description -----------------------------------------------------------
// NPCs found in and around Fortress Battlegrounds.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DUnderfortress69NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(42, 40120, "Statue of Goddess Vakarine", "d_underfortress_69", 1734.104, 491.8457, 498.2167, 45, "WARP_D_UNDERFORTRESS_69", "STOUP_CAMP", "STOUP_CAMP");
	}
}
