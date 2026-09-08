using System;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Zone.Buffs.Base;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;

namespace GuiltineSin.Zone.Buffs.Handlers
{
	/// <summary>
	/// Handler for card_MATK buff, which temporarily increases Magical Attack.
	/// Used by cards that provide Magical Attack +[★ * 1.5]% for duration.
	/// Scans all equipped cards to sum star levels for stacking.
	/// </summary>
	[BuffHandler(BuffId.card_MATK)]
	public class card_MATK : BuffHandler
	{
		private const float MultiplierPerStar = 1.5f / 100f;
		private const string ScriptFunction = "SCR_CARDEFFECT_ADD_BUFF_PC_RATE";

		public override void OnActivate(Buff buff, ActivationType activationType)
		{
			if (buff.Target is not Character character)
				return;

			var totalStarLevel = CardBuffHelper.GetTotalStarLevel(character, buff.Id, ScriptFunction);
			var bonus = totalStarLevel * MultiplierPerStar;

			SetPropertyModifier(buff, character, PropertyName.MATK_RATE_BM, bonus);
		}

		public override void OnEnd(Buff buff)
		{
			RemovePropertyModifier(buff, buff.Target, PropertyName.MATK_RATE_BM);
		}
	}
}
