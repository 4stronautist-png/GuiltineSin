using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers.Scouts.Thaumaturge
{
	[Package("laima")]
	[BuffHandler(BuffId.Quicken_Buff)]
	public class Quicken_BuffOverride : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var target = buff.Target;
			if (buff.Caster is not Character caster)
				return;

			var skillLevel = buff.NumArg1;

			if (buff.Vars.TryGetFloat("GuiltineSin.Skill.Quicken.PadLevel", out var padLevel))
			{
				skillLevel = padLevel;
			}

			var casterInt = caster.Properties.GetFloat(PropertyName.INT);
			var atkSpdBonus = (float)Math.Floor(100 + 10 * skillLevel + 0.5f * casterInt);

			AddPropertyModifier(buff, target, PropertyName.NormalASPD_BM, atkSpdBonus);

			if (target.Handle != caster.Handle)
			{
				Send.ZC_NORMAL.PlayTextEffect(target, caster, "SHOW_BUFF_TEXT", (float)buff.Id, null, "Item");
			}
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.NormalASPD_BM);
		}
	}
}
