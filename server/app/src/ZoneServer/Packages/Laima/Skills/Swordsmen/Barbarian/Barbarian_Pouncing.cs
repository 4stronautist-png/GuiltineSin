using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillUtilHelper;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using GuiltineSin.Zone.World.Actors.Monsters;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.Skills.Handlers.Barbarian
{
	/// <summary>
	/// Handler for the Barbarian skill Pouncing
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Barbarian_Pouncing)]
	public class Barbarian_PouncingOverride : IDynamicCasted
	{
		/// <summary>
		/// Called when the user starts casting the skill.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);
			caster.StartBuff(BuffId.Pouncing_Buff, TimeSpan.Zero);

			var padName = PadName.Barbarian_Pouncing;
			var length = 70f;

			// Barbarian41 uses a different pad, which has double range
			// but you can't move
			if (caster.IsAbilityActive(AbilityId.Barbarian41))
			{
				padName = PadName.Barbarian_Pouncing_Abil;
				length = 90f;
			}

			var pad = new Pad(padName, caster, skill, new Square(caster.Position, caster.Direction, length, 35));
			pad.Position = caster.Position;
			pad.Trigger.LifeTime = TimeSpan.FromMilliseconds(3500);
			pad.Movement.Speed = 100;

			caster.Map.AddPad(pad);
		}

		/// <summary>
		/// Called when the user stops casting the skill.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.StopBuff(BuffId.Pouncing_Buff);
		}
	}
}
