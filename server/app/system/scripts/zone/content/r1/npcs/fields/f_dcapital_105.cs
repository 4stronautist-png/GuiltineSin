//--- GuiltineSin Script ----------------------------------------------------------
// Baltinel Memorial
//--- Description -----------------------------------------------------------
// NPCs found in and around Baltinel Memorial.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FDcapital105NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(153231, "", "f_dcapital_105", -264.1042, 111.0862, -237.836, 0, "f_dcapital_105_elt", 2, 1);
	}
}
