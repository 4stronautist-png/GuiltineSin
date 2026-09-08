using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.World;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Thaumaturge
{
	[Package("laima")]
	[BuffHandler(BuffId.Transpose_Buff)]
	public class Transpose_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			var skillLevel = buff.NumArg1;

			var rate = Math.Min(1f, 0.40f + 0.04f * skillLevel);

			var currentStr = target.Properties.GetFloat(PropertyName.STR);
			var currentInt = target.Properties.GetFloat(PropertyName.INT);

			float strChange, intChange;

			if (target.TryGetActiveAbilityLevel(AbilityId.Thaumaturge9, out _))
			{
				var average = (currentStr + currentInt) / 2f;
				strChange = (average - currentStr) * rate;
				intChange = (average - currentInt) * rate;
			}
			else
			{
				strChange = (currentInt - currentStr) * rate;
				intChange = (currentStr - currentInt) * rate;
			}

			AddPropertyModifier(buff, target, PropertyName.STR_BM, strChange);
			AddPropertyModifier(buff, target, PropertyName.INT_BM, intChange);

			var hasBalanceAbility = target.TryGetActiveAbilityLevel(AbilityId.Thaumaturge9, out _);

			if (hasBalanceAbility || strChange == intChange)
			{
				target.PlayEffect("F_pc_status_str_up", 6, 1, EffectLocation.Bottom, 1);
				target.PlayEffect("F_pc_status_int_up", 6, 1, EffectLocation.Bottom, 1);
				buff.Vars.SetInt("EffectType", 0);
			}
			else if (strChange > intChange)
			{
				target.PlayEffect("F_pc_status_str_up", 6, 1, EffectLocation.Bottom, 1);
				buff.Vars.SetInt("EffectType", 1);
			}
			else
			{
				target.PlayEffect("F_pc_status_int_up", 6, 1, EffectLocation.Bottom, 1);
				buff.Vars.SetInt("EffectType", 2);
			}
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			RemovePropertyModifier(buff, target, PropertyName.STR_BM);
			RemovePropertyModifier(buff, target, PropertyName.INT_BM);

			var effectType = buff.Vars.GetInt("EffectType");

			if (effectType == 0)
			{
				target.PlayEffect("F_pc_status_str_up", 6, 1, EffectLocation.Bottom, 1);
				target.PlayEffect("F_pc_status_int_up", 6, 1, EffectLocation.Bottom, 1);
			}
			else if (effectType == 1)
			{
				target.PlayEffect("F_pc_status_int_up", 6, 1, EffectLocation.Bottom, 1);
			}
			else
			{
				target.PlayEffect("F_pc_status_str_up", 6, 1, EffectLocation.Bottom, 1);
			}
		}
	}
}
