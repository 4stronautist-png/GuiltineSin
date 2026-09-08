using System;
using System.Linq;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Wizards.Wizard
{
	/// <summary>
	/// Handler for the Wizard skill Teleportation.
	/// </summary>
	[SkillHandler(SkillId.Wizard_Teleportation)]
	public class Wizard_Teleportation : IGroundSkillHandler
	{
		private const float TeleportationDistance = 100;
		private static readonly TimeSpan ReUseTime = TimeSpan.FromSeconds(2);

		/// <summary>
		/// Handles skill, teleporting caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="farPos"></param>
		/// <param name="target"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			caster.StartBuff(BuffId.Teleportation_Buff, 0, 0, TimeSpan.FromSeconds(1), caster);
			caster.StartBuff(BuffId.Skill_NoDamage_Buff, 0, 0, TimeSpan.FromSeconds(1), caster);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, null);

			var now = DateTime.Now;
			var usedRecently = false;

			if (skill.Vars.TryGet("GuiltineSin.LastUse", out DateTime lastUse))
			{
				var elapsed = now - lastUse;
				usedRecently = elapsed < ReUseTime;
			}

			Position targetPos;
			if (usedRecently && skill.Vars.TryGet<Position>("GuiltineSin.LastPos", out var lastPos))
			{
				targetPos = lastPos;
			}
			else
			{
				targetPos = caster.Position.GetRelative(caster.Direction, TeleportationDistance);
				targetPos = caster.Map.Ground.GetLastValidPosition(caster.Position, targetPos);
			}

			skill.Vars.Set("GuiltineSin.LastPos", caster.Position);
			skill.Vars.Set("GuiltineSin.LastUse", now);

			caster.Position = targetPos;
			Send.ZC_SET_POS(caster, targetPos);
		}

		/// <summary>
		/// Returns the skill's max overheat count.
		/// </summary>
		/// <param name="skill"></param>
		/// <returns></returns>
		[SkillOverheatOverride(SkillId.Wizard_Teleportation)]
		public float GetOverheatMaxCount(Skill skill)
		{
			var result = 1;

			// Increase max overheat if "Teleportation: Return" is active,
			// which gives the user a brief window during which they can
			// teleport back to the position they teleported from.
			if (skill.Owner.IsAbilityActive(AbilityId.Wizard30))
				result = 2;

			return result;
		}
	}
}
