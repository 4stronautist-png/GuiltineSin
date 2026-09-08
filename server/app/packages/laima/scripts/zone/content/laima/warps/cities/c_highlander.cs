//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Highlander Master's Training Hall
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class c_highlanderWarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Highlander Master's Training Hall to Klaipeda
		AddWarp(2, "WS_HIGHLANDER_KLAPEDA", 270, From("c_highlander", -52, 192), To("c_Klaipe", 257, -134));
	}
}
