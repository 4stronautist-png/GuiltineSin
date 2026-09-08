//--- GuiltineSin Script ----------------------------------------------------------
// GuiltineSin Lua API
//--- Description -----------------------------------------------------------
// Provides QoL functions for client scripting.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using Yggdrasil.Scripting;

[Priority(100)]
public class GuiltineSinLuaApiScript : ClientScript
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
