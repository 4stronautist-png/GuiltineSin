//--- GuiltineSin Script ----------------------------------------------------------
// Map Initialization
//--- Description -----------------------------------------------------------
// Setups map specific tracks
//---------------------------------------------------------------------------

using System;
using GuiltineSin.Shared.Scripting;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Events;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Items;
using GuiltineSin.Zone.Events.Arguments;
public class MapInitializationScript : GeneralScript
{
	[On("PlayerReady")]
	public void OnPlayerReady(object sender, PlayerEventArgs args)
	{
		var character = args.Character;
		var map = character.Map;

		if (map == null)
			return;

		switch (map.Id)
		{
			//case
		}
	}
}

