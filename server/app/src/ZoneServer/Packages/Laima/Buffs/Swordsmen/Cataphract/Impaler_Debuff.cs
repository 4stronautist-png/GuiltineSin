using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.HandlersOverrides.Swordsmen.Cataphract
{
	/// <summary>
	/// Handler for the Impaler Debuff. Pins the target to the caster's spear
	/// and reduces their physical defense by 30%.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.Impaler_Debuff)]
	public class Impaler_DebuffOverride : BuffHandler
	{
		private const float DefenseReductionRate = 0.30f;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = buff.Caster;
			var target = buff.Target;

			target.AttachToObject(caster, "Dummy_Impaler", "None", holdAi: true);
			target.Lock(LockType.Attack);
			target.Lock(LockType.Movement);

			var currentDef = target.Properties.GetFloat(PropertyName.DEF);
			var reduction = -(currentDef * DefenseReductionRate);
			AddPropertyModifier(buff, target, PropertyName.DEF_BM, reduction);
		}

		public override void OnEnd(Buff buff)
		{
			var caster = buff.Caster;
			var target = buff.Target;

			RemovePropertyModifier(buff, target, PropertyName.DEF_BM);

			target.Unlock(LockType.Attack);
			target.Unlock(LockType.Movement);
			target.Position = caster.Position;
			Send.ZC_DETACH_TO_OBJ(target, caster);
		}
	}
}
