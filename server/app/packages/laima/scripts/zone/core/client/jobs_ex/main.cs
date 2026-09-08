//--- GuiltineSin Script ----------------------------------------------------------
// Jobs Expansion Script
//--- Description -----------------------------------------------------------
// Makes adjustments to job functions to make them more flexible.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class JobExClientScript : ClientScript
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
