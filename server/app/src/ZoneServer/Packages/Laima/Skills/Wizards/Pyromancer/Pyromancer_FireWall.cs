using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.Dialogues;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.Skills.Handlers.Pyromancer
{
	/// <summary>
	/// Handler for the Pyromancer skill Fire Wall.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Pyromancer_FireWall)]
	public class Pyromancer_FireWallOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int FireWallDuration = 15;
		private const int FireWallSize = 25;
		private const int BaseFireWallCount = 2;
		private const int FireWallCountPerLevel = 1;
		private const int FireWallPerRow = 6;

		/// <summary>
		/// Handles skill.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!skill.Vars.TryGet<Position>("GuiltineSin.ToolGroundPos", out var targetPos))
			{
				caster.ServerMessage(Localization.Get("No target location specified."));
				return;
			}

			if (!caster.InSkillUseRange(skill, targetPos))
			{
				caster.ServerMessage(Localization.Get("Too far away."));
				return;
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();

			Send.ZC_SKILL_READY(caster, skill, skillHandle, caster.Position, targetPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, caster.Handle, caster.Position, caster.Direction, targetPos);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos);

			this.ExecuteFireWall(caster, skill, targetPos);
		}

		/// <summary>
		/// Execute Fire Wall
		/// </summary>
		private void ExecuteFireWall(ICombatEntity caster, Skill skill, Position position)
		{
			var direction = caster.Position.GetDirection(position).AddDegreeAngle(90);
			var duration = TimeSpan.FromSeconds(FireWallDuration);
			var size = FireWallSize;
			var firewallCount = BaseFireWallCount + (skill.Level * FireWallCountPerLevel);

			var maxFirewallsPerRow = FireWallPerRow;
			var rows = (int)Math.Ceiling(firewallCount / (double)maxFirewallsPerRow);

			for (var row = 0; row < rows; row++)
			{
				var firewallsInThisRow = Math.Min(maxFirewallsPerRow, firewallCount - (row * maxFirewallsPerRow));
				var rowCenterPosition = position;

				// Adjust position for additional rows
				if (row > 0)
				{
					var rowOffset = row * size; // Move backwards
					rowCenterPosition = position.GetRelative(direction.AddDegreeAngle(90), rowOffset);
				}

				this.CreateFirewallRow(caster, skill, rowCenterPosition, direction, duration, size, firewallsInThisRow);
			}
		}

		/// <summary>
		/// Creates a row of Fire Wall pads.
		/// </summary>
		private void CreateFirewallRow(ICombatEntity caster, Skill skill, Position centerPosition, Direction direction, TimeSpan duration, float size, int wallCount)
		{
			for (var i = 0; i < wallCount; i++)
			{
				var effectPosition = centerPosition;

				if (wallCount > 1)
				{
					var offset = (i - (wallCount - 1) / 2.0f) * size;
					effectPosition = centerPosition.GetRelative(direction, offset);
				}

				this.CreateFirewallPad(caster, skill, effectPosition, direction, duration, size);
			}
		}

		/// <summary>
		/// Creates a single Fire Wall pad.
		/// </summary>
		private void CreateFirewallPad(ICombatEntity caster, Skill skill, Position position, Direction direction, TimeSpan duration, float size)
		{
			var pad = new Pad(PadName.Pyromancer_FireWall, caster, skill, new Circle(position, size));
			pad.Position = position;
			pad.Direction = direction;
			pad.Trigger.LifeTime = duration;
			pad.Trigger.MaxUseCount = 1;
			caster.Map.AddPad(pad);
		}
	}
}
