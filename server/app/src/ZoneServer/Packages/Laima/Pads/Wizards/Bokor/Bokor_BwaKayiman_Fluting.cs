using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using static GuiltineSin.Zone.Pads.Helpers.PadHelper;
using GuiltineSin.Zone.Skills.Combat;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Pads.Handlers
{
	/// <summary>
	/// Pad handler for Bwa Kayiman follower pads that form a conga line
	/// behind the caster, dealing trampling damage to enemies.
	/// </summary>
	[Package("laima")]
	[PadHandler(PadName.Bokor_BwaKayiman_Fluting)]
	public class Bokor_BwaKayiman_FlutingOverride : ICreatePadHandler, IDestroyPadHandler, IEnterPadHandler, IUpdatePadHandler
	{
		public void Created(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, true);
			pad.SetUpdateInterval(200);
		}
		public void Entered(object sender, PadTriggerActorArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var initiator = args.Initiator;
			var skill = pad.Skill;

			if (!creator.IsEnemy(initiator))
				return;

			var skillHitResult = SCR_SkillHit(creator, initiator, skill);
			initiator.StartBuff(BuffId.Pollution_Debuff, skill.Level, skillHitResult.Damage, TimeSpan.FromSeconds(3), creator);
		}

		public void Destroyed(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;

			Send.ZC_NORMAL.PadUpdate(pad, false);
		}

		public void Updated(object sender, PadTriggerArgs args)
		{
			var pad = args.Trigger;
			var creator = args.Creator;
			var skill = pad.Skill;

			if (!creator.IsCasting())
			{
				pad.Destroy();
				return;
			}

			if (creator is not Character character)
			{
				pad.Destroy();
				return;
			}

			if (pad.FollowTarget == null)
			{
				var summonHandle = pad.Variables.Get<int>("GuiltineSin.Skills.BwaKayiman.SummonHandle");
				var monster = character.Map.GetMonster(summonHandle);

				if (monster is not Summon summon || summon.IsDead)
				{
					pad.Destroy();
					return;
				}

				pad.FollowsTarget(summon);
			}
			else if (pad.FollowTarget.IsDead)
			{
				pad.Destroy();
				return;
			}

			var samdiveveMultiplier = 1f;
			if (creator.TryGetSkill(SkillId.Bokor_Samdiveve, out var samdiveveSkill))
				samdiveveMultiplier += samdiveveSkill.Level * 0.10f;

			var targets = pad.Map.GetAttackableEnemiesIn(creator, pad.Area);
			foreach (var actor in targets)
			{
				if (actor is not ICombatEntity target || target.IsDead)
					continue;

				if (!creator.IsEnemy(target))
					continue;

				var skillHitResult = SCR_SkillHit(creator, target, skill);
				var damage = skillHitResult.Damage * samdiveveMultiplier;

				target.TakeDamage(damage, creator);

				var hitInfo = new HitInfo(creator, target, skill, damage, skillHitResult.Result);
				Send.ZC_HIT_INFO(creator, target, hitInfo);
			}
		}
	}
}
