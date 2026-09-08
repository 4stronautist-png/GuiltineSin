//--- GuiltineSin Script ----------------------------------------------------------
// Unclutterer
//--- Description -----------------------------------------------------------
// Removes some of the clutter from the UI, such as cash shop buttons.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Versioning;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class UnclutterClientScript : ClientScript
{
	/// <summary>
	/// Loads script files.
	/// </summary>
	protected override void Load()
	{
		this.LoadAllScripts();
	}

	protected override void Ready(Character character)
	{
		var vars = character.Connection.Account.Variables;

		if (Versions.Protocol < 500)
			return;

		this.SendLuaScript(character, "001.lua");
		this.SendLuaScript(character, "002.lua");
	}
}
