//--- GuiltineSin Script ----------------------------------------------------------
// Gliehel Memorial
//--- Description -----------------------------------------------------------
// NPCs found in and around Gliehel Memorial.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FDcapital106NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153230, "", "f_dcapital_106", 392.5825, 71.17463, 988.9101, 21, "f_dcapital_106_elt", 2, 1);
	}
}
