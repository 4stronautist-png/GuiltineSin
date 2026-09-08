using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.CombatEntities.Components;
using GuiltineSin.Zone.World.Actors.Monsters;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Riding, Mounting Companion..
	/// </summary>
	[BuffHandler(BuffId.RidingCompanion)]
	public class RidingCompanion : BuffHandler
	{
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = (Companion)buff.Caster;
			var target = (Character)buff.Target;

			target.IsRiding = true;
			Send.ZC_NORMAL.PetIsInactive(target.Connection, caster);

			caster.StartBuff(BuffId.TakingOwner, TimeSpan.Zero, target);

			// Generic values from Velheider, should be companion specific?
			target.Properties.Modify(PropertyName.MSPD_BM, 3f);
			target.Properties.Modify(PropertyName.DR_BM, 3f);
			target.Properties.Modify(PropertyName.DEF_BM, 12f);

			Send.ZC_OBJECT_PROPERTY(target, PropertyName.MSPD, PropertyName.MSPD_BM,
						PropertyName.DR, PropertyName.DR_BM, PropertyName.MHP, PropertyName.MHP_RATE_BM,
						PropertyName.DEF, PropertyName.DEF_BM);
			Send.ZC_MOVE_SPEED(target);
		}

		public override void OnEnd(Buff buff)
		{
			var caster = (Companion)buff.Caster;
			var target = (Character)buff.Target;

			target.IsRiding = false;
			Send.ZC_NORMAL.PetIsInactive(target.Connection, caster);
			caster.RemoveBuff(BuffId.TakingOwner);

			// Remove mount-related buffs from the character (not the companion)
			// since these are self-buffs cast by the player
			target.RemoveBuff(BuffId.Trot_Buff);
			target.RemoveBuff(BuffId.AcrobaticMount_Buff);

			// Generic values from Velheider, should be companion specific?
			target.Properties.Modify(PropertyName.MSPD_BM, -3f);
			target.Properties.Modify(PropertyName.DR_BM, -3f);
			target.Properties.Modify(PropertyName.DEF_BM, -12f);

			Send.ZC_OBJECT_PROPERTY(target, PropertyName.MSPD, PropertyName.MSPD_BM,
						PropertyName.DR, PropertyName.DR_BM, PropertyName.MHP, PropertyName.MHP_RATE_BM,
						PropertyName.DEF, PropertyName.DEF_BM);
			Send.ZC_MOVE_SPEED(target);
		}
	}
}
