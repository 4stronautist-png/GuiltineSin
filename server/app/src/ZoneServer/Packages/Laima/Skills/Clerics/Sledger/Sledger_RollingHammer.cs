using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Sledger
{
	/// <summary>
	/// Handles Rolling Hammer.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Sledger_RollingHammer_Cleric)]
	public class Sledger_RollingHammerOverride : IGroundSkillHandler
	{
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!SledgerSkillHelper.TryStart(skill, caster, originPos, farPos, visualPos: caster.Position))
				return;

			var impactPos = caster.Position;
			var area = SledgerSkillHelper.CreateCircle(skill, caster, impactPos, radius: 42);
			SledgerSkillHelper.AttackArea(skill, caster, area, skill.Data.MultiHitCount, appliesBigBangReduction: true, bigBangReductionSeconds: 5);

			if (caster.IsAbilityActive(AbilityId.Sledger17))
			{
				caster.StartBuff(SledgerSkillHelper.RollingPowerBuffId, skill.Level, SledgerSkillHelper.RollingPowerFinalDamageBonus, SledgerSkillHelper.RollingPowerDuration, caster, skill.Id);
				caster.Components.Get<CooldownComponent>()?.Start(skill.Data.CooldownGroup, TimeSpan.FromMinutes(1));
				ApplyPowerStun(caster);
			}

			SledgerSkillHelper.RunCancellable(skill, async () =>
			{
				await skill.Wait(SledgerSkillHelper.GetSkillActionDuration(skill, 1150));
				SledgerSkillHelper.ResetSledgerAction(caster, skill);
			});
		}

		private static async void ApplyPowerStun(ICombatEntity caster)
		{
			await Task.Delay(SledgerSkillHelper.RollingPowerDuration);

			if (!caster.IsDead)
				caster.StartBuff(BuffId.Stun, TimeSpan.FromSeconds(2), caster);
		}
	}
}
