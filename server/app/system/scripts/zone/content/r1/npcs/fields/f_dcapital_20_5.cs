//--- GuiltineSin Script ----------------------------------------------------------
// Jeromel Park
//--- Description -----------------------------------------------------------
// NPCs found in and around Jeromel Park.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FDcapital205NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(157019, "", "f_dcapital_20_5", 929.8597, 294.7133, -722.7009, 0, "f_dcapital_20_5_elt", 2, 5);
	}
}
