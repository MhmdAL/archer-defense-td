using UnityEngine;
using System.Collections;

public class EnemyHealer : Monster {

	public float healCooldown;
	public float healRange;
	public float healPercent;
	CooldownTimer healCdTimer;

	public override void InitializeValues ()
	{
		base.InitializeValues ();
		healCdTimer = new CooldownTimer (healCooldown);
	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (healCdTimer.GetCooldownRemaining () <= 0) {
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, healRange);
			foreach (Collider2D c in cols) {
				Monster m = c.GetComponent<Monster> ();
				if (c != null && m != null && m != this) {
					m.CurrentHP += m.MaxHP.Value * healPercent;
					m.CurrentHP = Mathf.Clamp(m.CurrentHP, m.CurrentHP, m.MaxHP.Value);
				}
			}
			healCdTimer.ResetTimer (healCooldown);
		}
	}
}
