using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
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

namespace GuiltineSin.Zone.Skills.Handlers.QuarrelShooter
{
	/// <summary>
	/// Handler for the QuarrelShooter skill Block And Shoot.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.QuarrelShooter_BlockAndShoot)]
	public class QuarrelShooterBlockAndShootOverride : IGroundSkillHandler, IDynamicCasted
	{
		private const float BaseBlockBonus = 100f;
		private const float BlockBonusPerLevel = 10f;
		private const float PdefBlockBonusPercentPerLevel = 0.02f;

		private bool _isCasting;

		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			var blockBonus = this.CalculateBlockBonus(skill, caster);
			caster.StartBuff(BuffId.BlockAndShoot_Buff, skill.Level, blockBonus, TimeSpan.Zero, caster);
			_isCasting = true;
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.RemoveBuff(BuffId.BlockAndShoot_Buff);
			_isCasting = false;
		}

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

			skill.Run(this.HandleSkill(caster, skill));
		}

		private async Task HandleSkill(ICombatEntity caster, Skill skill)
		{
			while (_isCasting)
			{
				var position = caster.Position.GetRelative(caster.Direction, 30);
				_ = MonsterSkillPadFrontMissile(caster, skill, position, PadName.QuarrelShooter_BlockAndShoot, 300f, 1, 200f, 0f, 0f, 0);
				await skill.Wait(TimeSpan.FromMilliseconds(500));
			}
		}

		private float CalculateBlockBonus(Skill skill, ICombatEntity caster)
		{
			var baseBonus = BaseBlockBonus + (skill.Level * BlockBonusPerLevel);
			var pdefBonus = caster.Properties.GetFloat(PropertyName.DEF) * PdefBlockBonusPercentPerLevel * skill.Level;
			return (float)Math.Ceiling(baseBonus + pdefBonus);
		}
	}
}
