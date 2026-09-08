using System.Collections;
using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;

/// <summary>
/// Basic AI for most monsters.
/// </summary>
[Ai("BasicMonster")]
public class BasicMonsterAiScript : AiScript
{
	protected override void Setup()
	{
		this.MaxChaseDistance = 350;
		this.MaxRoamDistance = 1000;

		During("Idle", CheckEnemies);
		During("Idle", CheckFear);
		During("Attack", CheckTarget);
		During("Attack", CheckMaster);
		During("Attack", CheckFear);
	}

	protected override void Root()
	{
		StartRoutine("ReturnHome", ReturnHome());
	}
}
