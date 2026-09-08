using System;
using System.Collections.Generic;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Common
{
	/// <summary>
	/// Handler for the Link (de)buff, which links targets together to receive
	/// shared damage.
	/// </summary>
	[BuffHandler(BuffId.Link)]
	public class Link : BuffHandler
	{
		/// <summary>
		/// Applies link to the specified targets.
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="targets"></param>
		/// <param name="duration"></param>
		public static void Apply(ICombatEntity caster, IEnumerable<ICombatEntity> targets, TimeSpan duration)
		{
			foreach (var target in targets)
			{
				// As every target can only be part of one link, we'll first remove
				// any existing links they might have
				if (target.TryGetBuff(BuffId.Link, out var existingLink))
				{
					var existingTargets = existingLink.Vars.Get<IEnumerable<ICombatEntity>>("GuiltineSin.LinkMembers");
					foreach (var existingTarget in existingTargets)
						existingTarget.StopBuff(BuffId.Link);
				}

				// Then apply a new buff and remember the linked targets
				var linkBuff = target.StartBuff(BuffId.Link, 0, 0, duration, caster);
				linkBuff.Vars.Set("GuiltineSin.LinkMembers", targets);

				Send.ZC_NORMAL.PlayTextEffect(target, caster, "SHOW_BUFF_TEXT", (float)BuffId.Link, null, "Item");
			}
		}

		/// <summary>
		/// Breaks the target's current link.
		/// </summary>
		/// <param name="target"></param>
		public static void Remove(ICombatEntity target)
		{
			if (!target.TryGetBuff(BuffId.Link, out var linkBuff))
				return;

			// It's currently unclear whether this should break the full link or
			// only remove the target from the link. We'll simply break it for
			// now, since I don't feel like writing a workaround for locking
			// this shared target list to remove one of them.

			var linkTargets = linkBuff.Vars.Get<IEnumerable<ICombatEntity>>("GuiltineSin.LinkMembers");
			foreach (var linkTarget in linkTargets)
				linkTarget.StopBuff(BuffId.Link);
		}

		/// <summary>
		/// Applies the buff's effect during the combat calculations.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="modifier"></param>
		/// <param name="skillHitResult"></param>
		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Link)]
		public void OnAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!target.TryGetBuff(BuffId.Link, out var buff))
				return;

			if (!buff.Vars.TryGet<IEnumerable<ICombatEntity>>("GuiltineSin.LinkMembers", out var linkTargets))
				return;

			// Is the shared damage really the full amount? That would seem like
			// a lot to me, but who knows. TBD.
			var sharedDamage = (int)skillHitResult.Damage;

			foreach (var linkTarget in linkTargets)
			{
				if (linkTarget.Handle == target.Handle)
					continue;

				linkTarget.TakeSimpleHit(sharedDamage, attacker, skill.Id);
			}
		}
	}
}
