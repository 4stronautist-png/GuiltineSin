//--- GuiltineSin Script ----------------------------------------------------------
// Lemprasa Pond
//--- Description -----------------------------------------------------------
// NPCs found in and around Lemprasa Pond.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class Ep13FSiauliai1NpcScript : GeneralScript
{
	protected override void Load()
	{
		// Statue of Goddess Zemyna
		//-------------------------------------------------------------------------
		AddNpc(7, 40110, "Statue of Goddess Zemyna", "ep13_f_siauliai_1", 16.08543, 79.7736, 1297.291, 68, "SIAU16_SQ_06_EV_NPC", "SIAU16_SQ_06_EV_NPC", "SIAU16_SQ_06_EV_NPC");
	}
}
