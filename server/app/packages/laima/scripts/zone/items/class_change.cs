//--- GuiltineSin Script ----------------------------------------------------------
// Class Change Item Scripts
//--- Description -----------------------------------------------------------
// Item scripts that add class change points.
//---------------------------------------------------------------------------

using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class ClassChangeScripts : GeneralScript
{
	/// <summary>
	/// Adds class change points (for job switching).
	/// </summary>
	/// <param name="character"></param>
	/// <param name="item"></param>
	/// <param name="strArg">Item name for logging purposes</param>
	/// <param name="points">Number of class change points to add</param>
	/// <param name="numArg2"></param>
	/// <returns></returns>
	[ScriptableFunction]
	public ItemUseResult SCR_USE_Premium_RankReset_Point_Lv(Character character, Item item, string strArg, float points, float numArg2)
	{
		var pointsToAdd = (int)points;
		if (pointsToAdd <= 0)
			pointsToAdd = 100;

		// Add class change points to character's ETC properties
		var currentPoints = (int)character.Etc.Properties.GetFloat("ClassChangePoint", 0);
		character.SetEtcProperty("ClassChangePoint", currentPoints + pointsToAdd);

		// Play effect and show notification
		character.PlayEffect("F_sys_levelup_explosion", 1.5f, 1, EffectLocation.Bottom, 1);
		character.AddonMessage(AddonMessage.NOTICE_Dm_Scroll, ScpArgMsg("RankResetPointGet", "point", pointsToAdd.ToString()));

		return ItemUseResult.Okay;
	}
}
