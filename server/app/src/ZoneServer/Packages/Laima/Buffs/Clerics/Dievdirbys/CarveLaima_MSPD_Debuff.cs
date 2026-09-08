using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Buffs.Handlers;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Characters.Components;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Clerics.Dievdirbys
{
	/// <summary>
	/// Handler override for CarveLaima_MSPD_Debuff, which reduces movement speed
	/// by a fixed amount based on skill level, increased by Dievdirbys26 ability.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.CarveLaima_MSPD_Debuff)]
	public class CarveLaima_MSPD_DebuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Caster is not ICombatEntity caster) return;

			// The caster could be a statue; if so, get its owner.
			if (caster is IMonster monster)
			{
				caster.Map.TryGetCombatEntity(monster.OwnerHandle, out caster);
			}

			if (caster == null || !caster.TryGetSkill(SkillId.Dievdirbys_CarveLaima, out var skill))
				return;

			// Base MSPD reduction: 10 + skillLevel
			var mspdReduction = 10f + skill.Level;

			var SCR_Get_AbilityReinforceRate = ScriptableFunctions.Skill.Get("SCR_Get_AbilityReinforceRate");
			var byAbility = 1f + SCR_Get_AbilityReinforceRate(skill);

			mspdReduction *= byAbility;

			// Apply MSPD penalty
			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, -mspdReduction);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);
		}
	}
}
