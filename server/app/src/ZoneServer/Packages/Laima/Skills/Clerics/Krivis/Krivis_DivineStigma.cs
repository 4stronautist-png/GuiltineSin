using System;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Kriwi
{
	/// <summary>
	/// Handler for the Kriwi skill Divine Stigma.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Kriwi_DivineStigma)]
	public class Krivis_DivineStigmaOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const int DebuffDurationMilliseconds = 8000;

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
			var targetPos = originPos.GetRelative(caster.Direction, distance: 100);
			var area = new Yggdrasil.Geometry.Shapes.CircleF(targetPos, 150f);
			var targetList = caster.Map.GetAttackableEnemiesIn(caster, area);
			await skill.Wait(TimeSpan.FromMilliseconds(550));

			var spBonus = 0f;
			var currentSp = caster.Properties.GetFloat(PropertyName.SP);
			var spCost = currentSp * 0.10f;
			if (caster.TrySpendSp(spCost))
				spBonus = spCost;

			var modifier = new SkillModifier();
			modifier.BonusMAtk = spBonus;

			var targetCount = (skill.Level / 2) + 2;
			foreach (var currentTarget in targetList.Take(targetCount))
			{
				var splashHitResult = SCR_SkillHit(caster, currentTarget, skill, modifier);

				if (splashHitResult.Damage <= 0)
					continue;

				var duration = DebuffDurationMilliseconds;

				if (caster is Character character && character.TryGetActiveAbilityLevel(AbilityId.Kriwi19, out var abilLv))
				{
					// +1 second per ability level against demons
					if (currentTarget.Race == RaceType.Velnias)
						duration += 1000 * abilLv;
				}

				Send.ZC_NORMAL.SkillTargetAttachForce(caster, currentTarget, TimeSpan.FromSeconds(0.25), 1, "I_cleric_devinestigma_force_dark#Dummy_effect_lada", 0.5f, EffectLocation.Top);
				await skill.Wait(TimeSpan.FromMilliseconds(150));
				var stigmaDamageIncrease = 0.3f;
				currentTarget.StartBuff(BuffId.DivineStigma_Debuff, skill.Level, stigmaDamageIncrease, TimeSpan.FromMilliseconds(duration), caster);
				currentTarget.StartBuff(BuffId.Fire, skill.Level, splashHitResult.Damage, TimeSpan.FromMilliseconds(duration), caster);
			}
		}
	}
}
