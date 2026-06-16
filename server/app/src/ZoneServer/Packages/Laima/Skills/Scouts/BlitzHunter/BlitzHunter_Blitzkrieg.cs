using System;
using Melia.Shared.Game.Const;
using Melia.Shared.L10N;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills;
using Melia.Zone.Skills.Combat;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Scouts.BlitzHunter
{
	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_Blitzkrieg_Scout)]
	public class BlitzHunter_BlitzkriegOverride : BlitzHunter_BlitzkriegBase
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.BlitzHunter_Blitzkrieg_Archer)]
	public class BlitzHunter_BlitzkriegArcherOverride : BlitzHunter_BlitzkriegBase
	{
	}

	public class BlitzHunter_BlitzkriegBase : IMeleeGroundSkillHandler, ISelfSkillHandler
	{
		private static readonly TimeSpan Duration = TimeSpan.FromSeconds(10);
		private static readonly TimeSpan SelfCooldown = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan ActivationDelay = TimeSpan.FromMilliseconds(500);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, System.Collections.Generic.IList<ICombatEntity> targets)
			=> this.HandleCore(skill, caster);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
			=> this.HandleCore(skill, caster);

		private void HandleCore(Skill skill, ICombatEntity caster)
		{
			if (!caster.TryGetBuff(BuffId.VolticCatharsis_Buff, out var voltic) || voltic.OverbuffCounter < 70)
			{
				caster.ServerMessage(Localization.Get("You may not use this yet."));
				return;
			}

			var spMultiplier = caster.IsAbilityActive(AbilityId.BlitzHunter9) ? 1.5f : 1f;
			if (!BlitzHunterSkillHelper.TrySpendSkillSp(caster, skill, spMultiplier))
				return;

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			BlitzHunterSkillHelper.SendGroundSkillReady(caster, skill);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, caster.Position, ForceId.GetNew(), null);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			skill.Run(this.Activate(skill, caster));
		}

		private async System.Threading.Tasks.Task Activate(Skill skill, ICombatEntity caster)
		{
			await skill.Wait(ActivationDelay);

			BlitzHunterSkillHelper.ConsumeBlitzkriegStacks(caster, skill);
			caster.StartBuff(BuffId.Blitzkrieg_ColorBlend_Buff, skill.Level, 0, Duration, caster, skill.Id);
			caster.StartBuff(BuffId.Blitzkrieg_Buff, skill.Level, 0, Duration, caster, skill.Id);
			Send.ZC_NORMAL.Unknown_12_BlitzEffect(caster, "BodyAura_Electric_Blue_01", 1f, 1, -10f, -10f, 0f);
			Send.ZC_NORMAL.Unknown_12_BlitzEffect(caster, "BodyAura_Electric_Blue_01", 1f, 1, 10f, -10f, 0f);
			Send.ZC_NORMAL.Unknown_12_BlitzEffect(caster, "BodyAura_Electric_Blue_01", 1f, 1, 0f, -10f, 0f);
			Send.ZC_NORMAL.Unknown_12_BlitzEffect(caster, "BodyAura_PowerOverwhelming_Blue_01", 1f, 2, 0f, 10f, 0f);
			Send.ZC_NORMAL.Unknown_12_BlitzEffect(caster, "PowerUp_Electric_Blue_02", 2f, 2, 0f, 0f, 0f);
			skill.StartCooldown(SelfCooldown);
			await BlitzHunterSkillHelper.FinishCastAsync(skill, caster, ActivationDelay);
		}
	}
}
