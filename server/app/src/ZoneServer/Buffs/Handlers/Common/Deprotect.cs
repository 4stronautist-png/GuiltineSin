using System;
using GuiltineSin.Shared.ObjectProperties;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Monsters;
using Yggdrasil.Util;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Deprotect debuff, reducing physical defense by 20%.
	/// </summary>
	[BuffHandler(BuffId.UC_deprotect)]
	public class Deprotect : BuffHandler
	{
		private const float DefReductionRate = 0.2f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;

			// Monsters don't have DEF_RATE_BM, so calculate flat reduction instead
			if (target is Mob mob)
			{
				var defReduction = mob.Properties.GetFloat(PropertyName.DEF) * DefReductionRate;
				AddPropertyModifier(buff, target, PropertyName.DEF_BM, -defReduction);
			}
			else
			{
				AddPropertyModifier(buff, target, PropertyName.DEF_RATE_BM, -DefReductionRate);
			}
		}

		public override void OnEnd(Buff buff)
		{
			var target = buff.Target;

			if (target is Mob)
				RemovePropertyModifier(buff, target, PropertyName.DEF_BM);
			else
				RemovePropertyModifier(buff, target, PropertyName.DEF_RATE_BM);
		}
	}
}
