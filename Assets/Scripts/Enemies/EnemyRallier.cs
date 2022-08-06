using UnityEngine;
using System.Collections;

public class EnemyRallier : Monster {

	public float msBuffRange;
	public float msBuffValue;
	public float msBuffDuration;
	public float msBuffCooldown;
	public int msBuffStackCount;

	CooldownTimer msBuffCdTimer;

	public override void InitializeValues ()
	{
		base.InitializeValues ();
		msBuffCdTimer = new CooldownTimer (msBuffCooldown);
	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();
		if (msBuffCdTimer.GetCooldownRemaining () <= 0) {
			Collider2D[] cols = Physics2D.OverlapCircleAll (base.myTransform.position, msBuffRange);
			foreach (Collider2D c in cols) {
				Monster m = c.GetComponent<Monster> ();
				if (c != null && m != null && m != this) {
					m.AddModifier (new Modifier (msBuffValue, Name.EnemyRallier_MsIncrease, Type.MOVEMENT_SPEED, BonusOperation.Percentage,
						ValueStore.Instance.timerManagerInstance.StartTimer (msBuffDuration)), StackOperation.Additive, msBuffStackCount);
				}
			}
			msBuffCdTimer.ResetTimer (msBuffCooldown);
		}
	}
}
