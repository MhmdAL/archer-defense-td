using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyBoss3 : Monster {

	public float percentMissingHealthHeal;
	public float healCooldown;

	CooldownTimer healTimer = new CooldownTimer (0);

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (healTimer.GetCooldownRemaining () <= 0) {
			anim.SetBool ("HealGlow", true);

		}
	}

	public void TurnOffAnimation(){
	}

	public void ActivateHeal(){
		anim.SetBool ("HealGlow", false);
		CurrentHP += (MaxHP.Value - CurrentHP) * percentMissingHealthHeal;
		CurrentHP = Mathf.Clamp (CurrentHP, CurrentHP, MaxHP.Value);
		healTimer.ResetTimer (healCooldown);
	}
}
