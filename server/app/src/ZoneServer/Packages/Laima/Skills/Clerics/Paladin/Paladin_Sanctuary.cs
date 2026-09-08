using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Data.Database;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.L10N;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Pads;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills.Handlers.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using Yggdrasil.Geometry.Shapes;
using Yggdrasil.Util;
using static GuiltineSin.Zone.Skills.SkillUseFunctions;
using static GuiltineSin.Zone.Skills.Helpers.MonsterSkillHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillDamageHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillResultHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillTargetHelper;
using static GuiltineSin.Zone.Skills.Helpers.SkillUtilHelper;
using GuiltineSin.Zone.World.Actors.Pads;

namespace GuiltineSin.Zone.Skills.Handlers.Clerics.Paladin
{
	/// <summary>
	/// Handler for the Paladin skill Sanctuary.
	/// </summary>
	[Package("laima")]
	[SkillHandler(SkillId.Paladin_Sanctuary)]
	public class Paladin_SanctuaryOverride : IGroundSkillHandler, IDynamicCasted
	{
		public void StartDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			var pad = SkillCreatePad(caster, skill, caster.Position, 0f, PadName.Paladin_Sanctuary_Pad);
			skill.Vars.Set("Skill.Pad", pad);
		}

		public void EndDynamicCast(Skill skill, ICombatEntity caster, float maxCastTime)
		{
			// TODO: No Implementation SKL_CANCEL_CANCEL
			var pad = skill.Vars.Get<Pad>("Skill.Pad");
			pad.Destroy();
		}

		public void Handle(Skill skill, ICombatEntity caster, Position originPos, Position farPos, ICombatEntity target)
		{
			// Check if caster is wielding a shield
			if (caster is Character character)
			{
				var shield = character.Inventory.GetItem(EquipSlot.LeftHand);
				if (shield == null || shield.Data.EquipType1 != EquipType.Shield)
				{
					caster.ServerMessage(Localization.Get("Skill requires a shield."));
					Send.ZC_SKILL_DISABLE(caster);
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

			Send.ZC_SKILL_READY(caster, skill, originPos, farPos);
			Send.ZC_NORMAL.UpdateSkillEffect(caster, 0, caster.Position, caster.Direction, Position.Zero);
			Send.ZC_SKILL_MELEE_GROUND(caster, skill, farPos);

			// var targetPos = originPos.GetRelative(farPos);
			// var pad = SkillCreatePad(caster, skill, targetPos, 0f, PadName.Paladin_Sanctuary_Pad);
			// TODO: Implement Additional SP consumption
		}
	}
}
