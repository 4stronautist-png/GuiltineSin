using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using GuiltineSin.Zone.Skills.Combat;
using Yggdrasil.Geometry.Shapes;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Bokor
{
	/// <summary>
	/// Handler for the Bokor skill Hexing.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Bokor_Hexing)]
	public class Bokor_HexingOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int DebuffDurationMilliseconds = 20000;

		/// <summary>
		/// Handles skill behavior
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

			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.HandleSkill(caster, skill, originPos, farPos));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill, Position originPos, Position farPos)
		{
			var targetPos = caster.Position.GetRelative(caster.Direction, distance: 100);
			var circle = new CircleF(targetPos, 150f);
			var targets = caster.Map.GetAttackableEnemiesIn(caster, circle);
			await skill.Wait(TimeSpan.FromMilliseconds(350));

			var character = caster as Character;
			var summons = character?.Summons.GetSummons();

			var targetCount = 2 + skill.Level;
			foreach (var currentTarget in targets)
			{
				if (targetCount == 0)
					break;

				Send.ZC_NORMAL.SkillTargetAttachForce(caster, currentTarget, TimeSpan.FromSeconds(0.25), 1, "I_cleric_hexing_force_dark", 0.5f, EffectLocation.Top);

				await skill.Wait(TimeSpan.FromMilliseconds(150));

				currentTarget.StartBuff(BuffId.CurseOfWeakness_Debuff, skill.Level, 0, TimeSpan.FromMilliseconds(DebuffDurationMilliseconds), caster);

				if (summons != null)
				{
					foreach (var summon in summons)
					{
						summon.InsertHate(currentTarget, 300);
					}
				}

				targetCount--;
			}
		}
	}
}
