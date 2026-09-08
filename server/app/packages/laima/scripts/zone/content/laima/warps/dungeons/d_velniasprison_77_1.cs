//--- GuiltineSin Script ----------------------------------------------------------
// Warps
//--- Description -----------------------------------------------------------
// Sets up warps in Demon Prison District 5
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Maps;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class DVelniasprison771WarpsScript : GeneralScript
{
	protected override void Load()
	{
		// Tatenye Prison to Ruklys Street
		AddWarpPortal(From("d_velniasprison_77_1", -802, -1042), To("f_flash_61", -576, 1414));
	}
}
