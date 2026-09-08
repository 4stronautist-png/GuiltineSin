using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Items;

public class InstrumentItemScripts : GeneralScript
{
	private const string ActiveInstrumentTypeVar = "GuiltineSin.Instrument.Type";

	[ScriptableFunction]
	public ItemUseResult SCR_USE_ITEM_PLAY_SELECT_TOY_INSTRUMENT(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		if (string.IsNullOrWhiteSpace(strArg))
			return ItemUseResult.Fail;

		if (string.IsNullOrWhiteSpace(character.Variables.Temp.GetString(ActiveInstrumentTypeVar, "")))
			character.Lock(LockType.Movement);

		character.Variables.Temp.SetString(ActiveInstrumentTypeVar, strArg);
		character.StartBuff(BuffId.Instrument_Use_Buff, TimeSpan.Zero, character);

		Send.ZC_READY_INSTRUMENT(character, strArg, true);
		Send.ZC_ADDON_MSG(character, "INSTRUMENT_KEYBOARD_OPEN", 0, strArg);

		return ItemUseResult.OkayNotConsumed;
	}
}
