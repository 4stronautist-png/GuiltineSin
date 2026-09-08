using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.Barbarian
{
	/// <summary>
	/// Handler for the Barbarian skill Aggressor.
	/// </summary>
	[SkillHandler(SkillId.Barbarian_Aggressor)]
	public class Barbarian_Aggressor : ISelfSkillHandler
	{
		private const float BuffDurationBase = 20f;
		private const float BuffDurationPerLevel = 2f;

		/// <summary>
		/// Handles skill, applying the buff to the caster.
		/// </summary>
		/// <param name="skill"></param>
		/// <param name="caster"></param>
		/// <param name="originPos"></param>
		/// <param name="dir"></param>
		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}

			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			var target = caster;
			var buffDuration = BuffDurationBase + BuffDurationPerLevel * skill.Level;

			target.StartBuff(BuffId.Aggressor_Buff, skill.Level, 0, TimeSpan.FromSeconds(buffDuration), caster);

			Send.ZC_SKILL_MELEE_TARGET(caster, skill, target, null);
		}
	}
}
