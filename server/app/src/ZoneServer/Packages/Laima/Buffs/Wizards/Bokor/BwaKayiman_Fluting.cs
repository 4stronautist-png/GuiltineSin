using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.Skills;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Components;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handler for the BwaKayiman_Fluting buff applied to the caster during channeling.
	/// This buff indicates the caster is channeling Bwa Kayiman and increases movement speed.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.BwaKayiman_Fluting)]
	public class BwaKayiman_FlutingOverride : BuffHandler
	{
		private const float MovementSpeedBonus = 30;

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			buff.Target.AddState(StateType.Fluting);
			AddPropertyModifier(buff, buff.Target, PropertyName.MSPD_BM, MovementSpeedBonus);
		}

		public override void OnEnd(Buff buff)
		{
			buff.Target.RemoveState(StateType.Fluting);
			RemovePropertyModifier(buff, buff.Target, PropertyName.MSPD_BM);
		}
	}
}
