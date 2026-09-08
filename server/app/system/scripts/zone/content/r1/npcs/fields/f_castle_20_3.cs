//--- GuiltineSin Script ----------------------------------------------------------
// Inner Wall District 8
//--- Description -----------------------------------------------------------
// NPCs found in and around Inner Wall District 8.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle203NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157030, "", "f_castle_20_3", 467.1729, 139.2464, -586.9828, 0, "f_castle_20_3_elt", 2, 1);
	}
}
