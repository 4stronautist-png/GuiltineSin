//--- GuiltineSin Script ----------------------------------------------------
// Member Info Supplement
//--- Description -----------------------------------------------------------
// Complements the compare window with class icons and achievement counts.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors.Characters;

public class MemberInfoSupplementClientScript : ClientScript
{
	public const string ShowEquipmentVar = "GuiltineSin.MemberInfo.ShowEquipment";

	protected override void Load()
	{
		this.LoadAllScripts();
	}

	protected override void Ready(Character character)
	{
		this.SendAllScripts(character);
		this.SendState(character);
	}

	private void SendState(Character character)
	{
		Send.ZC_MEMBERINFO_VISIBILITY_UI(character);
	}
}
