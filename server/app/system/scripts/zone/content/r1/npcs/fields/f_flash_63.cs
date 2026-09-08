//--- GuiltineSin Script ----------------------------------------------------------
// Downtown
//--- Description -----------------------------------------------------------
// NPCs found in and around Downtown.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class FFlash63NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Track NPCs
		//---------------------------------------------------------------------------
		AddTrackNPC(147427, "", "f_flash_63", 897.8192, 233.7511, 654.0224, 357.82477, "flash_elevator", 2, 1);
	}
}
