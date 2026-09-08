using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.AetherBlader
{
	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Tidecall_Wizard)]
	public class AetherBlader_TideCallWizard : AetherBlader_TideCall
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Tidecall_Cleric)]
	public class AetherBlader_TideCallCleric : AetherBlader_TideCall
	{
	}

	[Package("laima")]
	[SkillHandler(SkillId.AetherBlader_Tidecall_Scout)]
	public class AetherBlader_TideCallScout : AetherBlader_TideCall
	{
	}

	public class AetherBlader_TideCall : ISelfSkillHandler, IDynamicCasted
	{
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.StartBuff(BuffId.AetherBlader_Blade_Buff, skill.Level, 0, System.TimeSpan.Zero, caster, skill.Id);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, true);
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.RemoveBuff(BuffId.AetherBlader_Blade_Buff);
			Send.ZC_NORMAL.UpdateAetherBladeLook(caster, false);
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!AetherBladerSkillHelper.TrySpendSkillSp(caster, skill))
				return;

			skill.IncreaseOverheat();
			caster.SetAttackState(true);
			Send.ZC_SKILL_READY(caster, skill, caster.Position, caster.Position);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Handle, caster.Position, dir, Position.Zero);
			Send.ZC_SKILL_MELEE_TARGET(caster, skill, caster, null);

			var finalDamageBonus = skill.Level * 0.01f;
			caster.StartBuff(BuffId.Tidecall_Buff, skill.Level, finalDamageBonus, AetherBladerSkillHelper.TideCallDuration, caster, skill.Id);
			AetherBladerSkillHelper.ExtendPuddles(caster, System.TimeSpan.FromSeconds(10 + skill.Level));
		}
	}
}
