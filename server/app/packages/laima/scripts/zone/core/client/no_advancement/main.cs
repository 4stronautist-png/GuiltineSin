//--- GuiltineSin Script ----------------------------------------------------------
// No Advancement
//--- Description -----------------------------------------------------------
// Disables the advancement button and screen, so player can no longer
// change jobs via the UI.
//---------------------------------------------------------------------------

using GuiltineSin.Zone;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class NoAdvancementClientScript : ClientScript
{
	protected override void Load()
	{
		this.LoadAllScripts();
	}

	protected override void Ready(Character character)
	{
		if (ZoneServer.Instance.Conf.World.NoAdvancement)
			this.SendAllScripts(character);
	}
}
