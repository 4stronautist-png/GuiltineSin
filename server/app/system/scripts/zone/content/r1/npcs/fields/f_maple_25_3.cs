//--- GuiltineSin Script ----------------------------------------------------------
// Lhadar Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Lhadar Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FMaple253NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157036, "", "f_maple_25_3", -917.34, 36.80652, -444.9715, 0, "f_maple_25_3_elt", 2, 1);
	}
}
