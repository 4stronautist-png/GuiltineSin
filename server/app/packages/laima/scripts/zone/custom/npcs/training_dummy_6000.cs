using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Scripting.Shortcuts;

public class GuildHangoutTrainingDummyScript : GeneralScript
{
	private const string MapClassName = "guild_agit_extension";
	private const float DummyMaxHp = 999999999f;

	protected override void Load()
	{
		const int swordPellId = 58014; // Monster_wood_carving, same pell used by the Highlander training room.

		AddPropertyOverrides(MapClassName, swordPellId, Properties(
			"MHP", DummyMaxHp,
			"MHP_BM", DummyMaxHp,
			"MINPATK", 0,
			"MAXPATK", 0,
			"MINMATK", 0,
			"MAXMATK", 0,
			"DEF", 500000000,
			"MDEF", 500000000,
			"DR", 0,
			"HR", 50000000,
			"CRTHR", 50000000,
			"BLK", 50000000,
			"BLK_BREAK", 50000000));

		if (!ZoneServer.Instance.World.TryGetMap(MapClassName, out var map))
			return;

		var dummy = new Mob(swordPellId, RelationType.Enemy)
		{
			Position = new Position(-1207.635f, 0.3334f, 2201.963f),
			Direction = new Direction(0),
			UniqueName = "GUILD_HANGOUT_TRAINING_DUMMY",
			Name = "Mister Paytoween",
		};

		dummy.Components.Add(new MovementComponent(dummy));
		dummy.Properties.Overrides.SetFloat(PropertyName.MHP, DummyMaxHp);
		dummy.Properties.Overrides.SetFloat(PropertyName.MHP_BM, DummyMaxHp);
		dummy.Properties.Overrides.SetFloat(PropertyName.MINPATK, 0);
		dummy.Properties.Overrides.SetFloat(PropertyName.MAXPATK, 0);
		dummy.Properties.Overrides.SetFloat(PropertyName.MINMATK, 0);
		dummy.Properties.Overrides.SetFloat(PropertyName.MAXMATK, 0);
		dummy.Properties.Overrides.SetFloat(PropertyName.DEF, 500000000);
		dummy.Properties.Overrides.SetFloat(PropertyName.MDEF, 500000000);
		dummy.Properties.Overrides.SetFloat(PropertyName.DR, 0);
		dummy.Properties.Overrides.SetFloat(PropertyName.HR, 50000000);
		dummy.Properties.Overrides.SetFloat(PropertyName.CRTHR, 50000000);
		dummy.Properties.Overrides.SetFloat(PropertyName.BLK, 50000000);
		dummy.Properties.Overrides.SetFloat(PropertyName.BLK_BREAK, 50000000);
		dummy.Properties.InvalidateAll();
		dummy.Properties.SetFloat(PropertyName.HP, DummyMaxHp);

		map.AddMonster(dummy);
	}
}
