using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_Lightningrod_Scout)]
	public class BlitzHunter_LightningLordOverride : BlitzHunter_LightningLordBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_Lightningrod_Archer)]
	public class BlitzHunter_LightningLordArcherOverride : BlitzHunter_LightningLordBase
	{
	}

	public class BlitzHunter_LightningLordBase : IMeleeGroundSkillHandler, ISelfSkillHandler
	{
		private static readonly TimeSpan Duration = TimeSpan.FromSeconds(10);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, System.Collections.Generic.IList<ICombatEntity> targets)
			=> this.HandleCore(skill, caster);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
			=> this.HandleCore(skill, caster);

		private void HandleCore(Skill skill, ICombatEntity caster)
		{
			var spMultiplier = caster.IsAbilityActive(AbilityId.BlitzHunter7) ? 1.3f : 1f;
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
				return;

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			BlitzHunterSkillHelper.SendGroundSkillReady(caster, skill);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, caster.Position, ForceId.GetNew(), null);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			caster.StartBuff(BuffId.Lightningrod_Buff, skill.Level, 0, Duration, caster, skill.Id);
			skill.Run(BlitzHunterSkillHelper.FinishCastAsync(skill, caster, TimeSpan.Zero));

			if (caster.TryGetActiveAbilityLevel(AbilityId.BlitzHunter7, out var abilityLevel))
			{
				var chance = abilityLevel * 10;
				if (RandomProvider.Get().Next(100) < chance)
					BlitzHunterSkillHelper.ApplyVolticCatharsis(caster, skill);
			}
		}
	}
}
