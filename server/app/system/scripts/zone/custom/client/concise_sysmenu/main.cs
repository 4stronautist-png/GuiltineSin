//--- GuiltineSin Script ----------------------------------------------------------
// Concise Sysmenu
//--- Description -----------------------------------------------------------
// Removes clutter from the system menu at the bottom right of the screen.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class ConciseSysmenuClientScript : ClientScript
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
