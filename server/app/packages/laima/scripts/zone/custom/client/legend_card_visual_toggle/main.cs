//--- GuiltineSin Script ----------------------------------------------------
// Legend Card Visual Toggle
//--- Description -----------------------------------------------------------
// Adds a small ON/OFF switch for legend card visuals in the card album UI.
//---------------------------------------------------------------------------

using GuiltineSin.Zone;
using GuiltineSin.Zone.Commands;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using Yggdrasil.Util.Commands;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class LegendCardVisualToggleClientScript : ClientScript
{
	protected override void Load()
	{
		this.LoadAllScripts();
		AddChatCommand("legendcardvisual", "<on|off|toggle|status>", "Toggles the equipped legend card visual effect.", 0, 99, HandleLegendCardVisual);
	}

	protected override void Ready(Character character)
	{
		this.SendLuaScript(character, "001.lua");
		character.Inventory.RefreshLegendCardVisual();
		this.SendState(character);
	}

	private CommandResult HandleLegendCardVisual(Character sender, Character target, string message, string commandName, Arguments args)
	{
		var available = sender.Inventory.TryGetAvailableLegendCardVisualId(out _);
		var enabled = sender.Inventory.IsLegendCardVisualEnabled();
		var action = args.Count > 0 ? args.Get(0).ToLowerInvariant() : "status";

		switch (action)
		{
			case "on":
				sender.Inventory.SetLegendCardVisualEnabled(available);
				break;
			case "off":
				sender.Inventory.SetLegendCardVisualEnabled(false);
				break;
			case "toggle":
				sender.Inventory.SetLegendCardVisualEnabled(available && !enabled);
				break;
			case "status":
				sender.Inventory.RefreshLegendCardVisual();
				break;
			default:
				return CommandResult.InvalidArgument;
		}

		ZoneServer.Instance.Database.SavePlayerData(sender, sender.Connection?.Account);
		this.SendState(sender);
		return CommandResult.Okay;
	}

	private void SendState(Character character)
	{
		var enabled = character.Inventory.IsLegendCardVisualActive();
		var available = character.Inventory.TryGetAvailableLegendCardVisualId(out _);
		Send.ZC_EXEC_CLIENT_SCP(character.Connection, $"LCV_SYNC({ToLuaBool(enabled)}, {ToLuaBool(available)})");
	}

	private static string ToLuaBool(bool value)
		=> value ? "true" : "false";
}
