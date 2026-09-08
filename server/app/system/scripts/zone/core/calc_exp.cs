//--- GuiltineSin Script ----------------------------------------------------------
// Exp Calculation Functions
//--- Description -----------------------------------------------------------
// Functions that calculate exp given by monsters.
//---------------------------------------------------------------------------

using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;

public class ExpCalculationsFunctionsScript : GeneralScript
{
	[ScriptableFunction]
	public float GET_EXP_RATIO(Mob monster, Character character)
	{
		return 1.0f;
	}
}
