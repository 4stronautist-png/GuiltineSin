using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Priest
{
	/// <summary>
	/// Handler for the Priest skill Sacrament.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Priest_Sacrament)]
	public class Priest_SacramentOverride : IGroundSkillHandler
	{
		private const int HolyWeaponBuffDurationSeconds = 300;
		private const float HolyWeaponBuffRadius = 100f;
		private const int DebuffDurationMilliseconds = 15000;
		private const float DebuffRadius = 100f;
		private const float DebuffHolyDamageBonusPerLevel = 0.01f;
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_READY(caster, skill, 1, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			skill.Run(this.HandleSkill(caster, skill));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill)
		{
			var targetPos = caster.Position;
			var targetCount = 6;
			caster.SetTargets(SkillSelectEnemiesInCircle(caster, targetPos, DebuffRadius, targetCount));
			var targets = caster.GetTargets();
			await skill.Wait(TimeSpan.FromMilliseconds(200));

			var damageBonus = skill.Level * DebuffHolyDamageBonusPerLevel;

			var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
			damageBonus *= 1f + SCR_Get_AbilityReinforceRate(skill);

			var hits = new List<SkillHitInfo>();
			foreach (var target in targets)
			{
				target.StartBuff(BuffId.Sacrament_Debuff, skill.Level, damageBonus, TimeSpan.FromMilliseconds(DebuffDurationMilliseconds), caster);

				var modifier = new SkillModifier();
				modifier.HitCount = 4;
				var skillHitResult = SCR_SkillHit(caster, target, skill, modifier);
				target.TakeDamage(skillHitResult.Damage, caster);

				var hit = new SkillHitInfo(caster, target, skill, skillHitResult, TimeSpan.Zero, TimeSpan.Zero);
				hits.Add(hit);
			}

			this.ApplyHolyWeaponBuff(skill, caster);

			Send.ZC_SKILL_HIT_INFO(caster, hits);
		}

		/// <summary>
		/// Enchants caster and party member weapons with holy.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		private void ApplyHolyWeaponBuff(Skill skill, ICombatEntity caster)
		{
			caster.StartBuff(BuffId.Sacrament_Buff, TimeSpan.FromSeconds(HolyWeaponBuffDurationSeconds));

			if (caster is Character character)
			{
				var party = character.Connection.Party;
				var members = caster.Map.GetPartyMembersInRange(character, HolyWeaponBuffRadius, true);

				if (party != null)
				{
					foreach (var member in members)
					{
						if (member == caster)
							continue;
						member.StartBuff(BuffId.Sacrament_Buff, TimeSpan.FromSeconds(HolyWeaponBuffDurationSeconds), caster);
					}
				}
			}
		}
	}
}
