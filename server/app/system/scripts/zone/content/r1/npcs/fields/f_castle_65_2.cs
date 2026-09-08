//--- GuiltineSin Script ----------------------------------------------------------
// Delmore Manor
//--- Description -----------------------------------------------------------
// NPCs found in and around Delmore Manor.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FCastle652NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(155108, "", "f_castle_65_2", -849.7505, 220.4114, 516.4036, 0, "f_castle_65_2_elt", 2, 1);
	}
}
