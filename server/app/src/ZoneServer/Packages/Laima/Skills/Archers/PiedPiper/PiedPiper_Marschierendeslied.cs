using System;
using System.Collections.Generic;
using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.PiedPiper
{
	[Package("laima")]
	[SkillHandler(SkillId.PiedPiper_Marschierendeslied)]
	public class PiedPiperMarschierendeslied : IGroundSkillHandler, IMeleeGroundSkillHandler
	{
		private const int DurationSeconds = 60;
		private const int BaseBlockCountAtMaxLevel = 20;

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
			=> this.Execute(skill, caster, originPos, farPos);

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, IList<ICombatEntity> targets)
			=> this.Execute(skill, caster, originPos, farPos);

		private void Execute(Skill skill, ICombatEntity caster, Position originPos, Position farPos)
		{
			if (!PiedPiperSkillHelper.TryStart(skill, caster, originPos, farPos))
				return;

			PiedPiperSkillHelper.SummonMouseFromSong(caster, skill);
			PiedPiperSkillHelper.SafePlayEffect(caster, "I_archer_Marschierendeslied", 3f);
			PiedPiperSkillHelper.SafePlaySound(caster, "skl_eff_piedpiper_marschierendeslied_melody");

			var allies = PiedPiperSkillHelper.GetPartyTargets(caster);
			var swordsmanBonus = allies.Count(IsSwordsmanTree);
			foreach (var ally in allies)
			{
				var count = BaseBlockCountAtMaxLevel + swordsmanBonus;
				var buff = PiedPiperSkillHelper.StartVisibleBuff(ally, BuffId.Marschierendeslied_Buff, skill.Level, count, TimeSpan.FromSeconds(DurationSeconds), caster, skill.Id);
				if (buff != null)
				{
					buff.OverbuffCounter = count;
					buff.NotifyUpdate();
				}

				PiedPiperSkillHelper.StartVisibleBuff(ally, BuffId.Allegro_Buff, skill.Level, 0, TimeSpan.FromSeconds(5), caster, skill.Id);

				if (caster.IsAbilityActive(AbilityId.PiedPiper8))
					ally.RemoveRandomDebuff(100);
			}
		}

		private static bool IsSwordsmanTree(ICombatEntity entity)
			=> entity is Character character && character.JobClass == JobClass.Swordsman;
	}
}
