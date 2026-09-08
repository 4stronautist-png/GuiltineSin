//--- GuiltineSin Script ----------------------------------------------------------
// Novaha Assembly Hall
//--- Description -----------------------------------------------------------
// NPCs found in and around Novaha Assembly Hall.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DAbbey641NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153102, "", "d_abbey_64_1", -1017.423, 366.9117, -465.3019, 0, "d_abbey_64_1_elt2", 3, 2);
	}
}
