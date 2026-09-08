//--- GuiltineSin Script ----------------------------------------------------------
// Martuis Storage Room
//--- Description -----------------------------------------------------------
// NPCs found in and around Martuis Storage Room.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DFiretower692NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157026, "", "d_firetower_69_2", -757.8323, 85.4979, 728.7798, 45, "d_firetower_69_2_elt", 2, 1);
	}
}
