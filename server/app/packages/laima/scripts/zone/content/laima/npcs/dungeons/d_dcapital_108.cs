//--- GuiltineSin Script ----------------------------------------------------------
// Pradzia Temple
//--- Description -----------------------------------------------------------
// NPCs found in and around Pradzia Temple.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DDcapital108NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Zemyna
		//-------------------------------------------------------------------------
		AddNpc(78, 40110, "Statue of Goddess Zemyna", "d_dcapital_108", -1606.255, 27.08374, -2953.66, -20, "D_DCAPITAL_108_ZEMINA", "D_DCAPITAL_108_ZEMINA", "D_DCAPITAL_108_ZEMINA");
	}
}
