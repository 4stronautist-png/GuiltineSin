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
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Monk
{
	/// <summary>
	/// Handler for the Monk skill Double Punch.
	/// Channeled skill that repeatedly punches enemies in front of
	/// the caster every 300ms (2 hits per cycle) while held.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Monk_DoublePunch)]
	public class Monk_DoublePunchOverride : IGroundSkillHandler, IDynamicCasted
	{

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);

			skill.Run(this.HandleSkill(skill, caster, originPos, farPos));
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			var endTime = DateTime.Now.AddMilliseconds(3500);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			await skill.Wait(TimeSpan.FromMilliseconds(200));

			while (caster.IsCasting(skill))
			{
				if (endTime <= DateTime.Now)
					break;

				if (!caster.TrySpendSp(skill))
				{
					caster.ServerMessage(Localization.Get("Not enough SP."));
					break;
				}

				await this.Attack(caster, skill);

				await skill.Wait(TimeSpan.FromMilliseconds(100));
			}

			Send.ZC_SKILL_DISABLE(caster);
		}

		private async Task Attack(ICombatEntity caster, Skill skill)
		{
			var targetArea = caster.Position.GetRelative(caster.Direction, 30);
			var casterPos = caster.Position;

			var splashParam = skill.GetSplashParameters(caster, casterPos, targetArea, length: 25, width: 20, angle: 0);
			var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			await SkillAttack(caster, skill, splashArea, 50, 30);

			await skill.Wait(TimeSpan.FromMilliseconds(100));

			splashParam = skill.GetSplashParameters(caster, casterPos, targetArea, length: 25, width: 20, angle: 0);
			splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
			await SkillAttack(caster, skill, splashArea, 80, 30);

			caster.StartBuff(BuffId.DoublePunch_Buff, skill.Level, 0, TimeSpan.FromSeconds(10), caster);
		}
	}
}
