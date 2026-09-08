//--- GuiltineSin Script ----------------------------------------------------------
// DOT Buff Items
//--- Description -----------------------------------------------------------
// Item scripts that heal or damage over time.
//---------------------------------------------------------------------------

using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

public class DotBuffItemScript : GeneralScript
{
	/// <summary>
	/// Used primarily by potions and other healing items, this function applies
	/// a specific buff to the character for a fixed duration.
	/// </summary>
	/// <param name="character">The character that used the item.</param>
	/// <param name="item">The item that was used.</param>
	/// <param name="buffName">The name of the buff to start, such as "Drug_HealHP".</param>
	/// <param name="numArg1">Usually the "heal factor" of the item.</param>
	/// <param name="numArg2">Unused.</param>
	/// <returns></returns>
	[ScriptableFunction]
	public ItemUseResult SCR_USE_ITEM_DotBuff(Character character, Item item, string buffName, float numArg1, float numArg2)
	{
		if (!Enum.TryParse<BuffId>(buffName, true, out var buffId))
		{
			character.ServerMessage(Localization.Get("Invalid buff '{0}', please report."), buffName);
			return ItemUseResult.Fail;
		}

		var duration = TimeSpan.FromSeconds(15);

		character.StartBuff(buffId, numArg1, numArg2, duration, character);
		return ItemUseResult.Okay;
	}
}
