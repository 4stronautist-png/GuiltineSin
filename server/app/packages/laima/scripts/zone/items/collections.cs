using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Scripting;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Events.Arguments;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;

public class CollectionItemScripts : GeneralScript
{
	[ScriptableFunction]
	public ItemUseResult SCR_USE_COLLECTION_GET(Character character, Item item, string strArg, float numArg1, float numArg2)
	{
		var collectionName = strArg;

		if (!ZoneServer.Instance.Data.CollectionDb.TryFindByClassName(collectionName, out var collectionData))
			return ItemUseResult.Fail;

		if (character.Collections.Has(collectionData.Id))
		{
			character.SystemMessage("AlreadyHaveCollection");
			return ItemUseResult.Fail;
		}

		if (character.Collections.Count == 0)
			character.ShowHelp("TUTO_COLLECTION");

		if (character.Collections.Add(collectionData.Id))
		{
			character.SystemMessage("GetCollection");

			Send.ZC_NORMAL.UnlockCollection(character, collectionData.Id);
			Send.ZC_ADDON_MSG(character, AddonMessage.UPDATE_READ_COLLECTION_COUNT, 0, null);

			return ItemUseResult.Okay;
		}

		return ItemUseResult.Fail;
	}

	[On("PlayerReady")]
	protected void OnPlayerReady(object sender, PlayerEventArgs e)
	{
		// Give rewards for collections to the character, in case there
		// are some they didn't get yet. This needs to be done on login,
		// but after the character, account, and connection were set up,
		// in case we need them.
		e.Character?.Collections?.GrantEligibleRewards();
	}
}
