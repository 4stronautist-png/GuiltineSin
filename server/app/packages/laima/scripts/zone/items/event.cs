//--- GuiltineSin Script ----------------------------------------------------------
// Event Items
//--- Description -----------------------------------------------------------
// This script handles the event items, such as the Event_Penalty buff and other event-related functionalities.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.Dialogues;
using GuiltineSin.Zone.Scripting.Shared;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Items;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class EventItemScript : GeneralScript
{
	[ScriptableFunction]
	public static ItemUseResult SCR_USE_ITEM_GODDESS_STATUE(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		var random = new Random();
		var distance = (float)(random.NextDouble() * 30 + 10);
		var spawnPosition = character.Position.GetRelative(character.Direction, distance);

		var npc = AddNpc(
			owner: character,
			className: "farm47_statue_zemina_small",
			position: spawnPosition,
			direction: -45,
			dialogFuncName: "GODDESS_STATUE_EV",
			tacticsFuncName: "GODDESS_STATUE_EV"
		);

		if (npc != null)
		{
			character.AddonMessage(AddonMessage.NOTICE_Dm_Exclaimation, ScpArgMsg("Goddess_Statue3"), 5);

			return ItemUseResult.Okay;
		}
		else
		{
			// Failed to create the NPC for some reason (e.g., invalid position).
			character.ServerMessage("Cannot place the statue here.");
			return ItemUseResult.Fail;
		}
	}

	[ScriptableFunction]
	public static ItemUseResult SCR_USE_ITEM_WHITECUBE(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		character.AddItem("Hat_628297", 1, "WHITE_HAIRACC");
		return ItemUseResult.Okay;
	}
}
