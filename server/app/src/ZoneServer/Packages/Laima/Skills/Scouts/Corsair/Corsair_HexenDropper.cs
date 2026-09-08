using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Corsair skill Hexen Dropper.
	/// Multi-hit forward dash attack with 7 hits.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Corsair_HexenDropper)]
	public class Corsair_HexenDropperOverride : IGroundSkillHandler
	{
		private const float SplashLength = 70f;
		private const float SplashWidth = 20f;
		private const float SplashAngle = 10f;
		private const int HitCount = 9;
		private const float JollyRogerDamageBonus = 0.2f;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.HandleSkill(skill, caster, originPos, farPos));
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			skill.Run(this.PlayDashEffects(skill, caster));

			var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: SplashLength, width: SplashWidth, angle: SplashAngle);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);

			var baseDelay = 200;
			for (var i = 0; i < HitCount; i++)
			{
				var aniTime = i == 0 ? 0 : 50;
				var hitDelay = baseDelay + (i * 50);
				await SkillAttack(caster, skill, splashArea, hitDelay, aniTime, null, this.ModifyDamage);
			}
		}

		private SkillHitResult ModifyDamage(Skill skill, ICombatEntity caster, ICombatEntity target, SkillHitResult skillHitResult)
		{
			if (target.IsBuffActive(BuffId.JollyRoger_Enemy_Debuff))
				skillHitResult.Damage *= 1 + JollyRogerDamageBonus;

			return skillHitResult;
		}

		private async Task PlayDashEffects(Skill skill, ICombatEntity caster)
		{
			var position = GetRelativePosition(PosType.Self, caster, caster, distance: 15, height: 20);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.6f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 20, angle: 0f, height: 15);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.5f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 20, angle: 572f, height: 20);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.6f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 15, angle: 0f, height: 15);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.7f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 15, angle: 859f, height: 15);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.6f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 15, height: 20);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.7f, 0f, 0f, caster.Direction.DegreeAngle);

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 20, angle: 0f, height: 15);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.5f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));

			position = GetRelativePosition(PosType.Self, caster, caster, distance: 20, angle: 572f, height: 20);
			caster.PlayGroundEffect(position, "I_warrior_florysh_shot_dash_ride_short", 0.6f, 0f, 0f, caster.Direction.DegreeAngle);
			await skill.Wait(TimeSpan.FromMilliseconds(100));
		}
	}
}
