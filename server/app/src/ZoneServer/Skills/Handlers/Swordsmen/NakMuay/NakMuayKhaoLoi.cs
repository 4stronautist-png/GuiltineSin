using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.Skills.SplashAreas;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;

namespace GuiltineSin.Zone.Skills.Handlers.Swordsmen.NakMuay
{
    /// <summary>
    /// Handler for the NakMuay skill Khao Loi.
    /// </summary>
    [SkillHandler(SkillId.NakMuay_KhaoLoi)]
    public class NakMuay_KhaoLoi : IGroundSkillHandler
    {
	    
	    private const float JumpDistance = 50;

        /// <summary>
        /// Handles usage of the skill.
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

	        if (caster.TryGetActiveAbility(AbilityId.NakMuay13, out _)) JumpToTarget(caster, target);

	        skill.IncreaseOverheat();
	        caster.SetAttackState(true);
	        
	        if (caster.TryGetSkill(SkillId.NakMuay_MuayThai, out var skillMuayThai)) 
		        skillMuayThai.ReduceCooldown(TimeSpan.FromSeconds(3));

	        Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
	        Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos, null);
	        
	        var splashParam = skill.GetSplashParameters(caster, originPos, farPos, length: 45, width: 40, angle: 10f);
	        var splashArea = skill.GetSplashArea(SplashType.Square, splashParam);
	        
	        skill.Run(this.Attack(skill, caster, splashArea));
        }
        
        private async Task Attack(Skill skill, ICombatEntity caster, ISplashArea splashArea)
        {
            var damageDelay = TimeSpan.FromMilliseconds(330);
            var skillHitDelay = skill.Properties.HitDelay;

            damageDelay /= skill.Properties.GetFloat(PropertyName.SklSpdRate);
            skillHitDelay /= skill.Properties.GetFloat(PropertyName.SklSpdRate);

            await skill.Wait(skillHitDelay);

            var hits = new List<SkillHitInfo>();
            var targets = caster.Map.GetAttackableEnemiesIn(caster, splashArea);

            foreach (var target in targets.LimitBySDR(caster, skill))
            {
	            var skillHitResult = SCR_NakSkillHit(caster, target, skill, SkillModifier.Default);
	            target.TakeDamage(skillHitResult.Damage, caster);
	            
	            if (caster.TryGetActiveAbility(AbilityId.NakMuay13, out _)) target.StartBuff(BuffId.Stun,
		            skill.Level, 0, TimeSpan.FromSeconds(1.5), caster);

	            var skillHit = new SkillHitInfo(caster, target, skill, skillHitResult, damageDelay, TimeSpan.Zero)
	            {
		            HitInfo = { ResultType = skillHitResult.Result }
	            };
	            hits.Add(skillHit);
            }
            Send.ZC_SKILL_HIT_INFO(caster, hits);
        }
        
        private void JumpToTarget(ICombatEntity caster, ICombatEntity target)
        {
	        if (target == null) return;
	        
	        var casterPos = caster.Position;
	        var targetPos = target.Position;

	        var jumpDest = casterPos.GetRelative(targetPos, JumpDistance);
	        var isValidDest = caster.Map.Ground.IsValidPosition(jumpDest);
	        if (!isValidDest)
		        return;

	        var dist = casterPos.Get2DDistance(jumpDest);
	        if (dist is <= 0 or > JumpDistance)
		        return;

	        caster.Position = jumpDest;
	        caster.TurnTowards(target);

	        Send.ZC_SET_POS(caster);
        }
    }
}
