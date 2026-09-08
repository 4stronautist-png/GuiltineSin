//--- GuiltineSin Script ----------------------------------------------------------
// Respawn Options Script
//--- Description -----------------------------------------------------------
// Modifies the respawn option display, to be more in line with what
// we expect of it for the server.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class RespawnOptionsClientScript : ClientScript
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
