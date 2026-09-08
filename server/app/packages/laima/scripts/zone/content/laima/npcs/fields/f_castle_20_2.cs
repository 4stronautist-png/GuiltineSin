//--- GuiltineSin Script ----------------------------------------------------------
// Inner Wall District 9
//--- Description -----------------------------------------------------------
// NPCs found in and around Inner Wall District 9.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle202NpcScript : GeneralScript
{
	protected override void Load()
	{
		
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157029, "", "f_castle_20_2", -1006.002, 1.829224, -609.7711, 16, "f_castle_20_2_elt", 2, 1);
		AddTrackNPC(157029, "", "f_castle_20_2", -894.3446, -120.3677, 549.2271, 0, "f_castle_20_2_elt2", 2, 1);

	}
}
