//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Fedimian Public House
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class c_request_1WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Fedimian Public House to Fedimian
		AddWarp("REQUEST1_FEDIMIAN", 90, From("c_request_1", 195, -130), To("c_fedimian", -778, -157));
	}
}
