//--- GuiltineSin Script ----------------------------------------------------------
// Ability Point Cost
//--- Description -----------------------------------------------------------
// Modifies the client's ability point cost function to return the server's
// configured value.
//---------------------------------------------------------------------------

using GuiltineSin.Zone;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;

public class AbilityPointCostClientScript : ClientScript
{
	protected override void Ready(Character character)
	{
		var exchangeRate = ZoneServer.Instance.Conf.World.AbilityPointCost;

		this.SendRawLuaScript(character, $@"
			GuiltineSin.Override(""GET_SILVER_BY_ONE_ABILITY_POINT_CALC"", function(original)
				return {exchangeRate};
			end)
		");
	}
}
