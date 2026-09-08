//--- GuiltineSin Script ----------------------------------------------------------
// Delmore Hamlet
//--- Description -----------------------------------------------------------
// NPCs found in and around Delmore Hamlet.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle651NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(155112, "", "f_castle_65_1", -728.6979, -8.345245, 768.4816, 0, "f_castle_65_1_elt", 2, 1);
	}
}
