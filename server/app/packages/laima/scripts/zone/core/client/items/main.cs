//--- GuiltineSin Script ----------------------------------------------------------
// Gem
//--- Description -----------------------------------------------------------
// Custom Items.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class ItemsClientScript : ClientScript
{
	protected override void Load()
	{
		this.LoadAllScripts();
	}

	protected override void Ready(Character character)
	{
		this.SendAllScripts(character);
	}
}
