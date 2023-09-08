using UnityEngine;
using System.Collections;

public class EnemyBoss6 : Monster {

	[Header("Values")]
	public float specialCooldown;
	public float speedBuffValue;
	public float speedBuffRange;
	public float speedBuffDuration;
	public float healValue;
	public float healRange;
	public float shieldCooldown;
	public float shieldDuration;

	CooldownTimer specialCooldownTimer;
	CooldownTimer shieldCooldownTimer;

	[Header("ShieldObject")]
	public GameObject shield;

	float multiplier = 1;

	public override void InitializeValues ()
	{
		base.InitializeValues ();

		specialCooldownTimer = new CooldownTimer (3);
		shieldCooldownTimer = new CooldownTimer (3);
	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (CurrentHP >= MaxHP.Value / 2) {
			multiplier = 1f;
		} else {
			multiplier = 1.5f;
		}

		if (shieldCooldownTimer.GetCooldownRemaining () <= 0) {
			anim.SetBool ("PlayShield", true);
		}

		if (specialCooldownTimer.GetCooldownRemaining () <= 0) {
			int random = Random.Range (1, 3);
			if (random == 1) {
				anim.SetBool ("PlayMS", true);

			} else if (random == 2) {
				anim.SetBool ("PlayHeal", true);
			}
			specialCooldownTimer.ResetTimer (specialCooldown);
		}
	}

	public void ActivateShield(){
		anim.SetBool("PlayShield", false);
		AddModifier (new Modifier (Name.EnemyBoss6_Shield, Type.Other, shieldDuration * multiplier,
			ApplyShieldModifier, DeApplyShieldModifier), StackOperation.Additive, 1);
		shieldCooldownTimer.ResetTimer (shieldCooldown);
	}

	public void ActivateMs(){
		anim.SetBool ("PlayMS", false);
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, speedBuffRange);
		foreach (Collider2D c in cols) {
			Monster m = c.GetComponent<Monster>();
			if (c != null && m != null && m != this) {
				m.AddModifier (new Modifier (speedBuffValue * multiplier, Name.EnemyRallier_MsIncrease, Type.MOVEMENT_SPEED, BonusType.Percentage,
					ValueStore.Instance.timerManagerInstance.StartTimer (speedBuffDuration)), StackOperation.Additive, 1);
			}
		}
	}

	public void ActivateHeal(){
		anim.SetBool ("PlayHeal", false);
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, healRange);
		foreach (Collider2D c in cols) {
			Monster m = c.GetComponent<Monster> ();
			if (c != null && m != null && c.gameObject != gameObject) {
				m.CurrentHP += m.MaxHP.Value * (healValue * multiplier);
				m.CurrentHP = Mathf.Clamp(m.CurrentHP, m.CurrentHP, m.MaxHP.Value);
			}
		}
	}

	public void DeactivateShieldAnimation(){
		anim.SetBool("PlayShield", false);
	}

	public void DeactivateMsAnimation(){
		anim.SetBool ("PlayMS", false);
	}

	public void DeactivateHealAnimation(){
		anim.SetBool ("PlayHeal", false);
	}

	public void ApplyShieldModifier(IModifiable m){
		shield.SetActive (true);
	}

	public void DeApplyShieldModifier(IModifiable m){
		shield.SetActive (false);
	}
}
