using System;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Quests;
using Yggdrasil.Logging;

namespace GuiltineSin.Zone.World.Quests.Objectives
{
	public class VisitLocationObjective : QuestObjective
	{
		public int TargetMapId { get; }
		public Position TargetPosition { get; set; }
		public float TargetRadius { get; set; }

		public VisitLocationObjective(int targetMapId, Position targetPosition, float targetRadius)
		{
			this.TargetMapId = targetMapId;
			this.TargetPosition = targetPosition;
			this.TargetRadius = targetRadius;
			this.TargetCount = 1;
		}

		public override void Load()
		{
			base.Load();
			Log.Debug($"VisitLocationObjective type loaded. Player-centric checking will be used.");
		}

		public bool IsPositionWithinObjective(Position characterPosition)
		{
			return characterPosition.Get3DDistance(this.TargetPosition) <= this.TargetRadius + 1f;
		}
	}
}
