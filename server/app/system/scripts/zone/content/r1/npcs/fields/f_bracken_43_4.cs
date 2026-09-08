//--- GuiltineSin Script ----------------------------------------------------------
// Mollogheo Forest
//--- Description -----------------------------------------------------------
// NPCs found in and around Mollogheo Forest.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FBracken434NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153181, "", "f_bracken_43_4", 665.4066, 59.12576, 327.8538, 0, "f_bracken_43_4_elt", 2, 1);
	}
}
