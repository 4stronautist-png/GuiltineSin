using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using GuiltineSin.Zone.Skills.Helpers;

namespace GuiltineSin.Zone.Skills.Handlers.Scouts.Rogue
{
	/// <summary>
	/// Handler for the Rogue skill Lachrymator.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Rogue_Lachrymator)]
	public class Rogue_LachrymatorOverride : IGroundSkillHandler, IDynamicCasted
	{

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			var isEscape = caster.IsAbilityActive(AbilityId.Rogue27);
			var targetPos = caster.Position;

			if (!isEscape)
			{
				if (!skill.Vars.TryGet<Position>("GuiltineSin.ToolGroundPos", out targetPos))
				{
					caster.ServerMessage(Localization.Get("No target location specified."));
					return;
				}
			}

			if (!caster.TrySpendSp(skill))
			{
				caster.ServerMessage(Localization.Get("Not enough SP."));
				return;
			}
			skill.IncreaseOverheat();
			caster.SetAttackState(true);

			Send.ZC_SKILL_MELEE_GROUND(caster, skill, targetPos);

			skill.Run(this.HandleSkill(skill, caster, targetPos, isEscape));
		}

		private async Task HandleSkill(Skill skill, ICombatEntity caster, Position targetPos, bool isEscape)
		{
			await skill.Wait(TimeSpan.FromMilliseconds(250));
			var value = PadName.lachrymator_pad;
			if (caster.IsAbilityActive(AbilityId.Rogue9))
				value = PadName.Rogue_Lachrymator_abil;
			if (caster.IsAbilityActive(AbilityId.Rogue26))
				value = PadName.lachrymator_pad_Rogue26;

			if (isEscape)
			{
				SkillCreatePad(caster, skill, caster.Position, 0f, value);
				caster.StartBuff(BuffId.Sprint_Buff, 10, 0f, TimeSpan.FromSeconds(1), caster);
			}
			else
			{
				await MissilePadThrow(skill, caster, targetPos, new MissileConfig
				{
					Effect = new EffectConfig("I_archer_Lachrymator_force_mash#Bip01 R Hand", 0.6f),
					EndEffect = new EffectConfig("I_bomb003_dark", 1f),
					DotEffect = EffectConfig.None,
					Range = 0f,
					FlyTime = 0.5f,
					DelayTime = 0f,
					Gravity = 300f,
					Speed = 1f,
					HitTime = 200f,
					HitCount = 0,
					GroundEffect = EffectConfig.None,
					GroundDelay = 0f,
					EffectMoveDelay = 0f,
				}, 0f, value);
			}
		}
	}
}
