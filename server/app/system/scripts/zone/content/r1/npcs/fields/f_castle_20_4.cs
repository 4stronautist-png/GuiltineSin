//--- GuiltineSin Script ----------------------------------------------------------
// City Wall District 8
//--- Description -----------------------------------------------------------
// NPCs found in and around City Wall District 8.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle204NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157025, "", "f_castle_20_4", 1157.179, 126.4701, 271.2875, 0, "d_castle_20_4_elt", 2, 1);
	}
}
