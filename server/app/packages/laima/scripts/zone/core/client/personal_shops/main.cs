//--- GuiltineSin Script ----------------------------------------------------------
// Personal Shops
//--- Description -----------------------------------------------------------
// Personal Shops aren't officially supported anymore, and their scripts
// are partially broken. These client scripts fix them to a certain degree,
// to make the UI usable again.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class PersonalShopsClientScript : ClientScript
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
