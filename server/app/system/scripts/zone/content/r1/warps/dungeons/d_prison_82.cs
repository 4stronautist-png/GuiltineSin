//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Investigation Room
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class d_prison_82WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Investigation Room to Workshop
		AddWarp("PRISON82_PRISON81", 179, From("d_prison_82", -930, -25), To("d_prison_81", 618, -739));
	}
}
