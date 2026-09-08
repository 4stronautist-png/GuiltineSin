using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Thaumaturge
{
	[Package("laima")]
	[BuffHandler(BuffId.ShrinkBody_Debuff)]
	public class ShrinkBody_DebuffOverride : BuffHandler
	{
		private const float BaseHpReductionPercent = 10f;
		private const float HpReductionPercentPerLevel = 1f;
		private const float MaxHpReductionPercent = 30f;
		private const float HpReductionPerInt = 4f;
		private const float MoveSpeedReduction = 10f;
		private const float AtkReductionPercent = 0.20f;
		private const float AtkReductionPerAbilityLevel = 0.02f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			if (!buff.Vars.Has("GuiltineSin.ShrinkBody.Size"))
				buff.Vars.Set("GuiltineSin.ShrinkBody.Size", target.EffectiveSize);

			var currentScale = target.Properties.GetFloat(PropertyName.Scale);
			if (!buff.Vars.Has("GuiltineSin.ShrinkBody.Scale"))
				buff.Vars.Set("GuiltineSin.ShrinkBody.Scale", currentScale > 0 ? currentScale : 1f);

			switch (target.EffectiveSize)
			{
				case SizeType.L:
					target.Properties.SetString(PropertyName.Size, SizeType.M);
					(target as Mob)?.InvalidateSizeCache();
					target.ChangeScale(2f / 3f, 0.2f);
					break;
				case SizeType.M:
					target.Properties.SetString(PropertyName.Size, SizeType.S);
					(target as Mob)?.InvalidateSizeCache();
					target.ChangeScale(2f / 3f, 0.2f);
					break;
				case SizeType.S:
					target.ChangeScale(2f / 3f, 0.2f);
					break;
			}

			var skillLevel = buff.NumArg1;
			var casterInt = buff.NumArg2;

			var hpReductionPercent = Math.Min(MaxHpReductionPercent, BaseHpReductionPercent + HpReductionPercentPerLevel * skillLevel);
			var targetMhp = target.Properties.GetFloat(PropertyName.MHP);
			var hpReduction = targetMhp * (hpReductionPercent / 100f) + casterInt * HpReductionPerInt;
			hpReduction = Math.Min(hpReduction, targetMhp - 1);

			AddPropertyModifier(buff, target, PropertyName.MHP_BM, -hpReduction);

			if (target.Hp > target.MaxHp)
				target.Properties.SetFloat(PropertyName.HP, target.MaxHp);

			Send.ZC_UPDATE_ALL_STATUS(target, 0);

			var minPAtk = target.Properties.GetFloat(PropertyName.MINPATK);
			var maxPAtk = target.Properties.GetFloat(PropertyName.MAXPATK);
			var patk = (minPAtk + maxPAtk) / 2;

			var minMAtk = target.Properties.GetFloat(PropertyName.MINMATK);
			var maxMAtk = target.Properties.GetFloat(PropertyName.MAXMATK);
			var matk = (minMAtk + maxMAtk) / 2;

			// Thaumaturge2: Increase ATK reduction by 2% per ability level
			var atkReduction = AtkReductionPercent;
			if (buff.Caster is ICombatEntity caster && caster.TryGetActiveAbilityLevel(AbilityId.Thaumaturge2, out var abilityLevel))
				atkReduction += AtkReductionPerAbilityLevel * abilityLevel;

			AddPropertyModifier(buff, target, PropertyName.PATK_BM, -(patk * atkReduction));
			AddPropertyModifier(buff, target, PropertyName.MATK_BM, -(matk * atkReduction));

			AddPropertyModifier(buff, target, PropertyName.MSPD_BM, -MoveSpeedReduction);
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			var originalSize = buff.Vars.Get("GuiltineSin.ShrinkBody.Size", target.EffectiveSize);
			var originalScale = buff.Vars.Get("GuiltineSin.ShrinkBody.Scale", 1f);

			target.Properties.SetString(PropertyName.Size, originalSize);
			(target as Mob)?.InvalidateSizeCache();
			target.ChangeScale(originalScale, 0.2f);

			RemovePropertyModifier(buff, target, PropertyName.MHP_BM);
			RemovePropertyModifier(buff, target, PropertyName.PATK_BM);
			RemovePropertyModifier(buff, target, PropertyName.MATK_BM);
			RemovePropertyModifier(buff, target, PropertyName.MSPD_BM);

			Send.ZC_UPDATE_ALL_STATUS(target, 0);
		}
	}
}
