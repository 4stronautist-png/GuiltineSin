//--- GuiltineSin Script ----------------------------------------------------------
// Kill Events
//--- Description -----------------------------------------------------------
// Events that occur when certain entites are killed.
//---------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GuiltineSin.Shared.Game.Const;
using GuiltineSin.Shared.Scripting;
using GuiltineSin.Zone.Events.Arguments;
using GuiltineSin.Zone.Network;
using GuiltineSin.Zone.Scripting;
using GuiltineSin.Zone.World.Actors;
using GuiltineSin.Zone.World.Actors.Characters;
using GuiltineSin.Zone.World.Actors.Monsters;
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
		attacker.StartBuff(BuffId.RootCrystalMoveSpeed, 3, 0, TimeSpan.FromMinutes(10), attacker);
		attacker.StartBuff(BuffId.RootCrystalCoolDown_BUFF, 0, 0, TimeSpan.FromMinutes(1), attacker);

		if (attacker is Character character)
			CallSafe(this.MonsterHealStamina(mob, character, 100000));
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
