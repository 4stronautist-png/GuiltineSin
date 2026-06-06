using System;
using Melia.Shared.Game.Const;
using Melia.Shared.L10N;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.Skills.Helpers;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_LightningStep_Scout)]
	public class BlitzHunter_StormstrideOverride : BlitzHunter_StormstrideBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_LightningStep_Archer)]
	public class BlitzHunter_StormstrideArcherOverride : BlitzHunter_StormstrideBase
	{
	}

	public class BlitzHunter_StormstrideBase : IMeleeGroundSkillHandler, IGroundSkillHandler
	{
		private const float FallbackDashDistance = 35f;
		private static readonly TimeSpan OverfluxDuration = TimeSpan.FromSeconds(10);
		private static readonly TimeSpan OverfluxLockDuration = TimeSpan.FromSeconds(20);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, System.Collections.Generic.IList<ICombatEntity> targets)
			=> this.HandleCore(skill, caster, originPos, farPos, targets != null && targets.Count > 0 ? targets[0] : null);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
			=> this.HandleCore(skill, caster, originPos, farPos, target);

		private void HandleCore(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (caster.IsBuffActive(BuffId.Overflux_Limit_Debuff))
			{
				caster.ServerMessage(Localization.Get("You may not use this yet."));
				SkillUseHelper.SkillCancelCancel(caster, skill);
				return;
			}

			var spMultiplier = caster.IsAbilityActive(AbilityId.BlitzHunter6) ? 1.3f : 1f;
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
			{
				SkillUseHelper.SkillCancelCancel(caster, skill);
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var direction = BlitzHunterSkillHelper.GetDirection(caster, originPos, farPos);
			var from = caster.Position;
			var maxDashDistance = skill.Properties.GetFloat(PropertyName.MaxR);
			if (maxDashDistance <= 0f)
				maxDashDistance = FallbackDashDistance;

			var requestedDistance = (float)caster.Position.Get2DDistance(farPos);
			var dashDistance = requestedDistance > 0f
				? Math.Min(requestedDistance, maxDashDistance)
				: maxDashDistance;

			var destination = caster.Position.GetRelative(direction, dashDistance);
			destination = caster.Map.Ground.GetLastValidPosition(caster.Position, destination);

			BlitzHunterSkillHelper.SendGroundSkillReady(caster, skill);
			caster.Position = destination;
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, destination, ForceId.GetNew(), null);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, target?.Handle ?? 0, destination, direction, Position.Zero);
			Send.ZC_MOVE_POS(caster, from, destination, 260, 0.15f);
			skill.Run(BlitzHunterSkillHelper.FinishCastAsync(skill, caster, TimeSpan.Zero));

			var currentStacks = 0;
			if (caster.TryGetBuff(BuffId.Overflux_Debuff, out var overflux))
				currentStacks = overflux.OverbuffCounter;

			var newStacks = Math.Min(5, currentStacks + 1);
			caster.StartBuff(BuffId.Overflux_Debuff, skill.Level, 0, OverfluxDuration, caster, skill.Id, buff =>
			{
				buff.OverbuffCounter = newStacks;
			});

			if (newStacks >= 5)
			{
				caster.StartBuff(BuffId.Overflux_Limit_Debuff, skill.Level, 0, OverfluxLockDuration, caster, skill.Id);
				skill.StartCooldown(OverfluxLockDuration);
			}
		}
	}
}
