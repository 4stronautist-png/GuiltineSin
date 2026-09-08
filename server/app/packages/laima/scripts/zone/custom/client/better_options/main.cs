//--- GuiltineSin Script ----------------------------------------------------------
// Better (Default) Options
//--- Description -----------------------------------------------------------
// Sets certain options to absolutely objectively better default values.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class BetterOptionsClientScript : ClientScript
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

		if (vars.Perm.ActivateOnce("GuiltineSin.ClientScripts.BetterOptions.DoneFirstTime"))
			this.SendLuaScript(character, "001.lua");
	}
}
