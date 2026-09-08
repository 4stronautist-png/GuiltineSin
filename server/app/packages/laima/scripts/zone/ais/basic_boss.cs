using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Scripting.AI;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using Yggdrasil.Extensions;
using Yggdrasil.Logging;

/// <summary>
/// AI for Boss monsters
/// </summary>
[Ai("BasicBoss")]
public class BasicBossAi : AiScript
{
	protected override void Setup()
	{
		// Configure boss-specific parameters
		this.MaxChaseDistance = 400;
		this.MaxRoamDistance = 1500;
		this.SetAggroRange(400f);
		this.SetHatePerSecond(30, 5);

		// Set up behavior checks
		During("Idle", CheckEnemies);
		During("Attack", CheckTarget);
		During("Attack", CheckMaster);
	}

	protected override void Root()
	{
		StartRoutine("ReturnHome", ReturnHome());
	}
}
