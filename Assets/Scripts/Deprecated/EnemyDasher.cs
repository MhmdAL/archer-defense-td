using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDasher : Monster {

	int shotNumber;
	public int shotsNeeded;
	public float dashCooldown;
	public float dashSpeed;
	public float dashDuration;

	CooldownTimer dashCooldownTimer = new CooldownTimer (0);

	private List<MyTimer> timers = new List<MyTimer>();

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (dashCooldownTimer.GetCooldownRemaining () <= 0 && !InCombat && shotNumber > 0) {
			anim.SetBool ("Dash", true);
			// FixedMovespeed (dashSpeed, dashDuration * shotNumber);

			MyTimer t = ValueStore.Instance.timerManagerInstance.StartTimer (dashDuration * shotNumber);
			t.TimerElapsed += OnDashEnd;
			timers.Add (t);
	
			shotNumber = 0;
			dashCooldownTimer.ResetTimer (dashCooldown);
		}
	}

	public void OnDashEnd(){
		anim.SetBool ("Dash", false);
	}

	public override void Damage (float damage, float armorpen, DamageSource source, IAttacking killer, DamageMetaData damageMeta)
	{
		base.Damage (damage, armorpen, source, killer, damageMeta);
		if (killer is Tower) {
			shotNumber += 1;
		}
	}

	void OnDestroy(){
		foreach (var item in timers) {
			item.TimerElapsed -= OnDashEnd;
		}
	}
}
