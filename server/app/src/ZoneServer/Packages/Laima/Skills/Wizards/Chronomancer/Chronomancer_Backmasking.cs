using System;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Pads;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Chronomancer
{
	[Package("laima")]
	[SkillHandler(SkillId.Chronomancer_BackMasking)]
	public class Chronomancer_BackmaskingOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			this.RefreshFriendlyPads(caster, skill, farPos);
		}

		private void RefreshFriendlyPads(ICombatEntity caster, Skill skill, Position position)
		{
			var range = 300;
			var maxPads = 6;

			var pads = caster.Map.GetPads(pad =>
				pad.Creator is ICombatEntity padCreator
				&& !padCreator.IsEnemy(caster)
				&& pad != null
				&& !pad.IsDead
				&& pad.Position.Get2DDistance(position) <= range
			).OrderBy(pad => pad.Position.Get2DDistance(position)).Take(maxPads);

			foreach (var pad in pads)
			{
				pad.Trigger.ResetLifeTime();
			}
		}
	}
}
