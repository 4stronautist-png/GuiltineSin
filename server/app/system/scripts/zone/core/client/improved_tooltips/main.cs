//--- GuiltineSin Script ----------------------------------------------------------
// Improved Tooltips
//--- Description -----------------------------------------------------------
// Adds or improves some tooltips on default UI controls.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class ImprovedTooltipsClientScript : ClientScript
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
