using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;

namespace GuiltineSin.Zone.Skills.Handlers.Rodelero
{
	/// <summary>
	/// Handler for the Rodelero skill Shield Charge.
	/// Channeled charge with shield that knocks down nearby enemies.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Rodelero_ShieldCharge)]
	public class Rodelero_ShieldChargeOverride : IGroundSkillHandler, IDynamicCasted
	{
		/// <summary>
		/// Called when the skill begins channeling.
		/// </summary>
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			caster.RemoveBuff(BuffId.ShieldCharge_Buff);
			caster.StartBuff(BuffId.ShieldCharge_Buff, skill.Level, 0f, TimeSpan.Zero, caster);
			var targetPos = caster.Position.GetRelative(caster.Direction, distance: 20f);
			SkillCreatePad(caster, skill, targetPos, 0f, PadName.Rodelero_ShieldCharge);
			caster.PlaySound("voice_archer_camouflage_shot", "voice_archer_m_camouflage_shot");
		}

		/// <summary>
		/// Called when the skill channeling ends.
		/// </summary>
		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			caster.RemoveBuff(BuffId.ShieldCharge_Buff);
			SkillRemovePad(caster, skill);
			caster.StopSound("voice_archer_camouflage_shot", "voice_archer_m_camouflage_shot");
		}

		/// <summary>
		/// Handles the Shield Charge skill execution.
		/// </summary>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var skillHandle = ZoneServer.Instance.World.CreateSkillHandle();
			Send.ZC_SKILL_READY(caster, skill, skillHandle, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);
		}
	}
}
