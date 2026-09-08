using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using Yggdrasil.Logging;
using System.Linq;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Shared.Data.Database;

namespace GuiltineSin.Zone.Skills.Handlers.Priest
{
	/// <summary>
	/// Handler for the Priest skill Resurrection.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Priest_Resurrection)]
	public class ResurrectionOverride : IGroundSkillHandler
	{
		/// <summary>
		/// Handles skill
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		/// <param name="targets"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			var length = 110;
			var width = 60;
			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: length, width: width, angle: 10f);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			var allies = caster.Map.GetDeadAlliedEntitiesIn(caster, splashArea);
			foreach (var ally in allies)
			{
				if (ally is Character player)
				{
					player.Resurrect(ResurrectOptions.TryAgain);
				}
			}
		}
	}
}
