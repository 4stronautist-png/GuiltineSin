using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using Yggdrasil.Util;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Scripting.ScriptableEvents;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Corsair
{
	/// <summary>
	/// Handler for the Looting Buff.
	/// Party members with this buff have a chance to steal silver from enemies on hit.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Looting_Buff)]
	public class Looting_BuffOverride : BuffHandler
	{
		private const int BaseSilverAmount = 5;
		private const int SilverPerLevel = 3;
		private const float DropChance = 15f;

		private const string LootingCounterKey = "GuiltineSin.Buff.Looting.DropCount";
		private const int MaxDropsPerMob = 10;

		[CombatCalcModifier(CombatCalcPhase.AfterCalc, BuffId.Looting_Buff)]
		public void OnAttackAfterCalc(ICombatEntity attacker, ICombatEntity target, Skill skill, SkillModifier modifier, SkillHitResult skillHitResult)
		{
			if (!attacker.TryGetBuff(BuffId.Looting_Buff, out var buff))
				return;

			if (skillHitResult.Damage == 0)
				return;

			if (attacker is not Character character)
				return;

			if (target is not Mob mob)
				return;

			if (mob.Rank == MonsterRank.NPC || mob.Rank == MonsterRank.Material || mob.Rank == MonsterRank.MISC)
				return;

			var dropCount = mob.Vars.GetInt(LootingCounterKey);
			if (dropCount >= MaxDropsPerMob)
				return;

			var rnd = RandomProvider.Get();
			if (rnd.NextDouble() * 100 < DropChance)
			{
				mob.Vars.SetInt(LootingCounterKey, dropCount + 1);

				var silverAmount = BaseSilverAmount + (buff.NumArg1 * SilverPerLevel);
				var mobLevelMultiplier = 1f + (mob.Level * 0.005f);
				mob.DropItem(character, (int)ItemId.Silver, (int)(silverAmount * mobLevelMultiplier), 100);
			}
		}
	}
}
