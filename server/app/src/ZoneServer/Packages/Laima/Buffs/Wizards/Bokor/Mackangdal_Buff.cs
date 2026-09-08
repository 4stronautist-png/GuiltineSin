using System;
using System.Linq;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Wizards.Bokor
{
	/// <summary>
	/// Handler for the Mackangdal buff.
	/// Redirects damage taken to caster's summons.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Mackangdal_Buff)]
	public class Mackangdal_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
		}

		public override void OnEnd(Buff buff)
		{
		}

		public override void WhileActive(Buff buff)
		{
		}

		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Mackangdal_Buff)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Mackangdal_Buff, out var buff))
				return;

			if (!(target is Character character))
				return;

			if (!character.Skills.TryGet(SkillId.Bokor_Mackangdal, out var mackangdalSkill))
				return;

			var summonsList = character.Summons.GetSummons().Where(s => !s.IsDead).ToList();
			if (summonsList.Count == 0)
				return;

			var redirectRate = 0.40f + (0.03f * mackangdalSkill.Level);
			var damageToRedirect = skillHitResult.Damage * redirectRate;
			var damagePerSummon = damageToRedirect / summonsList.Count;
			var totalRedirectedDamage = 0f;

			foreach (var summon in summonsList)
			{
				var summonCurrentHp = summon.Hp;
				var damageToSummon = Math.Min(damagePerSummon, summonCurrentHp);

				summon.TakeDamage(damageToSummon, attacker);

				var hitInfo = new HitInfo(attacker, summon, damageToSummon, HitResultType.Hit);
				Send.ZC_HIT_INFO(attacker, summon, hitInfo);

				totalRedirectedDamage += damageToSummon;
			}

			skillHitResult.Damage -= totalRedirectedDamage;
		}
	}
}
