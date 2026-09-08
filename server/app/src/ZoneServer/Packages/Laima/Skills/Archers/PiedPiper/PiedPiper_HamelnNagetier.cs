using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Skills.Handlers.Archers.PiedPiper
{
	[Package("laima")]
	[SkillHandler(SkillId.PiedPiper_HamelnNagetier)]
	public class PiedPiperHamelnNagetier : IPassiveSkillHandler, ISelfSkillHandler, ISkillCombatAttackAfterCalcHandler
	{
		public const string LastTargetVar = "GuiltineSin.PiedPiper.BestFriend.LastTarget";

		public void Handle(Skill skill, ICombatEntity caster)
		{
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Direction dir)
		{
		}

		public void OnAttackAfterCalc(Skill skill, ICombatEntity attacker, ICombatEntity target, Skill attackerSkill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (attacker is not Character character || target == null || target.IsDead || skillHitResult.Damage <= 0)
				return;

			if (attackerSkill == null || attackerSkill.Data.ClassName.Contains("DOT", System.StringComparison.OrdinalIgnoreCase) || attackerSkill.Data.ClassName.Contains("Dot", System.StringComparison.OrdinalIgnoreCase))
				return;

			character.Variables.Temp.SetInt(LastTargetVar, target.Handle);
		}
	}
}
