using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers.Monster;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Buffs.Handlers.Laima.Monster
{
	/// <summary>
	/// Handler for Mythic_Chain_Lightning_Buff.
	/// When the monster is attacked, it retaliates with chain lightning
	/// hitting nearby enemies. Capped at 1 lightning per second.
	/// Uses Electrocute-style chain visual effect.
	/// Damage is 5% of monster's MATK per target, applied via SCR_HIT calc.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Mythic_Chain_Lightning_Buff)]
	public class Mythic_Chain_Lightning_BuffOverride : BuffHandler
	{
		private const string LastLightningTimeVar = "GuiltineSin.Mythic.LastLightning";
		private const string ChainEffect = "I_laser005_blue#Dummy_effect_electrocute";
		private const float ChainEffectScale = 4f;
		private const float ChainDuration = 0.1f;
		private const float BounceRange = 150f;
		private const float DamageRate = 0.05f;
		private const int MaxBounces = 5;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target is not Mob monster)
				return;

			MythicBuffHelper.ApplyMythicStats(monster);
			buff.Vars.Set(LastLightningTimeVar, 0L);
		}

		public override void OnEnd(Buff buff)
		{
		}

		/// <summary>
		/// When the mythic monster takes damage, fire chain lightning at nearby enemies.
		/// </summary>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Mythic_Chain_Lightning_Buff)]
		public void OnDefenseAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Mythic_Chain_Lightning_Buff, out var buff))
				return;

			if (target is not Mob monster || target.Map == null)
				return;

			var now = Environment.TickCount64;
			if (!buff.Vars.TryGet<long>(LastLightningTimeVar, out var lastTime))
				lastTime = 0;
			if (now - lastTime < 1000)
				return;

			buff.Vars.Set(LastLightningTimeVar, now);

			_ = this.FireChainLightning(monster, attacker);
		}

		/// <summary>
		/// Fires chain lightning from the monster, bouncing between nearby enemies.
		/// </summary>
		private async Task FireChainLightning(Mob monster, ICombatEntity initialTarget)
		{
			if (monster.IsDead || monster.Map == null)
				return;

			var chainTargets = new List<ICombatEntity>();
			var hitKeyActorList = new List<(int, int)>();
			var alreadyHit = new HashSet<ICombatEntity>();

			var currentTarget = initialTarget;
			ICombatEntity previousTarget = null;

			for (var bounce = 0; bounce < MaxBounces; bounce++)
			{
				if (currentTarget == null || currentTarget.IsDead)
					break;

				if (!monster.IsEnemy(currentTarget))
					break;

				var key = monster.GenerateSyncKey();
				hitKeyActorList.Add((currentTarget.Handle, key));
				chainTargets.Add(currentTarget);
				alreadyHit.Add(currentTarget);

				var nearbyTargets = currentTarget.Map.GetAttackableEnemiesInPosition(monster, currentTarget.Position, BounceRange)
					.Where(t => t != currentTarget && !t.IsDead);

				var nextTarget = nearbyTargets
					.Where(t => !alreadyHit.Contains(t))
					.OrderBy(t => t.Position.Get2DDistance(currentTarget.Position))
					.FirstOrDefault();

				if (nextTarget == null)
				{
					nextTarget = nearbyTargets
						.Where(t => t != previousTarget)
						.OrderBy(t => t.Position.Get2DDistance(currentTarget.Position))
						.FirstOrDefault();
				}

				previousTarget = currentTarget;
				currentTarget = nextTarget;

				if (nextTarget == null)
					break;
			}

			if (chainTargets.Count == 0)
				return;

			monster.PlayChainEffect(ChainEffect, ChainEffectScale, ChainDuration, hitKeyActorList.ToArray());

			var dummySkill = new Skill(monster, SkillId.Normal_Attack);

			foreach (var chainTarget in chainTargets)
			{
				if (monster.IsDead || chainTarget.IsDead)
					continue;

				var skillHitResult = SCR_SkillHit(monster, chainTarget, dummySkill);
				var damage = skillHitResult.Damage * DamageRate;

				chainTarget.TakeDamage(damage, monster);

				var hitInfo = new HitInfo(monster, chainTarget, dummySkill, damage, skillHitResult.Result);
				Send.ZC_HIT_INFO(monster, chainTarget, hitInfo);

				await Task.Delay(100);
			}
		}
	}
}
