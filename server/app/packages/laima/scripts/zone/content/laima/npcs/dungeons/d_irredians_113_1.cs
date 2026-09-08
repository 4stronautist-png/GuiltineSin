//--- GuiltineSin Script ----------------------------------------------------------
// Irredian Shelter
//--- Description -----------------------------------------------------------
// NPCs found in and around Irredian Shelter.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DIrredians1131NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Vakarine
		//-------------------------------------------------------------------------
		AddNpc(6, 40120, "Statue of Goddess Vakarine", "d_irredians_113_1", -517.5229, 92.14448, -981.6158, 54, "WARP_D_IRREDIANS113_1", "STOUP_CAMP", "STOUP_CAMP");
	}
}
