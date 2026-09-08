//--- GuiltineSin Script ----------------------------------------------------------
// Dadan Jungle
//--- Description -----------------------------------------------------------
// NPCs found in and around Dadan Jungle.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FBracken633NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153095, "", "f_bracken_63_3", -252.8264, 189.2868, -89.98244, 111, "f_bracken_63_3_elt", 2, 1);
	}
}
