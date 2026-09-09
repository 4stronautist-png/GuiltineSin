using System;
using Melia.Shared.Game.Const;
using Melia.Shared.Packages;
using Melia.Shared.World;
using Melia.Zone.Network;
using Melia.Zone.Skills.Handlers.Base;
using Melia.Zone.World.Actors;

namespace Melia.Zone.Skills.Handlers.Wizards.Bonemancer
{
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneShield_Swordman)] public class Bonemancer_BoneShieldSwordman : Bonemancer_BoneShield { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneShield_Wizard)] public class Bonemancer_BoneShieldWizard : Bonemancer_BoneShield { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneShield_Archer)] public class Bonemancer_BoneShieldArcher : Bonemancer_BoneShield { }
	[Package("laima"), SkillHandler(SkillId.Bonemancer_BoneShield_Cleric)] public class Bonemancer_BoneShieldCleric : Bonemancer_BoneShield { }

	public class Bonemancer_BoneShield : ISelfSkillHandler
	{
		private static readonly TimeSpan NormalDuration = TimeSpan.FromMinutes(30);
		private static readonly TimeSpan UnifiedStructureDuration = TimeSpan.FromSeconds(30);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction direction)
		{
			if (!BonemancerSkillHelper.TrySpendSp(caster, skill))
				return;
			skill.IncreaseOverheat();

			Send.ZC_SKILL_READY(caster, skill, caster.Position, caster.Position);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, 0, originPos, direction, Position.Zero);

			caster.RemoveBuff(BuffId.BoneShield_Buff);
			caster.RemoveBuff(BuffId.BoneShield_Abil_Buff);
			if (caster.IsAbilityActive(AbilityId.Bonemancer8))
				caster.StartBuff(BuffId.BoneShield_Abil_Buff, skill.Level, 0, UnifiedStructureDuration, caster, skill.Id);
			else
				caster.StartBuff(BuffId.BoneShield_Buff, skill.Level, 0, NormalDuration, caster, skill.Id);
		}
	}
}
