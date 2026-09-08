using System;
using GuiltineSin.Shared.Packages;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.Skills.Combat;
using GuiltineSin.Zone.Skills;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handle for the Mass Heal, HP Recovery.
	/// </summary>
	[Package("laima")]
	[BuffHandler(BuffId.MassHeal_Buff)]
	public class MassHeal_BuffOverride : BuffHandler
	{
		/// <summary>
		/// Starts the buff, healing the target.
		/// </summary>
		/// <param name="buff"></param>
		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			var caster = buff.Caster;
			var target = buff.Target;
			var skillId = buff.SkillId;
			var healAmount = buff.NumArg2;

			target.Heal(healAmount, 0);
		}
	}
}
