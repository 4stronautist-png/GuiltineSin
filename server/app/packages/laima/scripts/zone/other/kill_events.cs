//--- GuiltineSin Script ----------------------------------------------------------
// Kill Events
//--- Description -----------------------------------------------------------
// Events that occur when certain entites are killed.
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Scripting;
using GuiltineSin.Zone.Events.Arguments;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
using Yggdrasil.Geometry.Shapes;
using static GuiltineSin.Shared.Util.TaskHelper;

public class KillEventsScript : GeneralScript
{
	[On("EntityKilled")]
	public void OnEntityKilled(object sender, CombatEventArgs args)
	{
		if (args.Target is Mob mob && mob.Data.Faction == FactionType.RootCrystal)
			this.OnRootCrystalKilled(mob, args.Attacker);
	}

	private void OnRootCrystalKilled(Mob mob, ICombatEntity attacker)
	{
		var duration = TimeSpan.FromSeconds(15);
		var applied = new HashSet<ICombatEntity>();

		void ApplyBuff(ICombatEntity target)
		{
			if (target == null || target.IsDead || !applied.Add(target))
				return;
			target.StartBuff(BuffId.RootCrystalMoveSpeed, 10, 0, duration, attacker);
		}

		ApplyBuff(attacker);

		var character = mob.GetKillBeneficiary(attacker);
		if (character == null)
			return;

		ApplyBuff(character);
		CallSafe(this.MonsterHealStamina(mob, character, 100000));

		var recipients = new List<Character> { character };
		if (character.Connection?.Party != null)
			recipients.AddRange(character.Map.GetPartyMembersInRange(character, 150).Where(m => m != character));

		foreach (var recipient in recipients)
		{
			if (recipient != character)
			{
				ApplyBuff(recipient);
				CallSafe(this.MonsterHealStamina(mob, recipient, 100000));
			}

			var area = new CircleF(recipient.Position, 150);
			foreach (var minion in recipient.Map.GetAliveAlliedEntitiesIn(recipient, area).Where(e => e is Summon || e is Companion))
				ApplyBuff(minion);
		}
	}

	private async Task MonsterHealStamina(Mob mob, Character character, int staminaAmount)
	{
		await Task.Delay(200);

		character.Properties.Stamina += staminaAmount;

		// Officials don't seem to send ZC_STAMINA, but for some reason
		// the stamina doesn't update if we don't do that.
		Send.ZC_ACTION_PKS(character, mob, 0, 2, 75);
		Send.ZC_MON_STAMINA(character, mob, staminaAmount);
		Send.ZC_STAMINA(character, character.Properties.Stamina);

		await Task.Delay(700);

		character.PlayEffectLocal("F_staup", 1, EffectLocation.Top);
	}
}
