using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EnemyTunneler : Monster {

	public float counterAttackCooldown;

	public Enemy15CounterAttack counterAttackPrefab;

	private List<Enemy15CounterAttack> counterObjects;

	CooldownTimer counterTimer;

    public override void InitializeValues ()
	{
		base.InitializeValues ();

		counterTimer = new CooldownTimer (counterAttackCooldown);

		counterObjects = GetComponentsInChildren<Enemy15CounterAttack> ().ToList();
		foreach (var item in counterObjects) {
			item.gameObject.SetActive (false);
		}
	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();

		if (counterTimer.GetCooldownRemaining () <= 0 && targetedBy.Count > 0) {
			anim.SetBool ("PlayCounterAttack", true);
			counterTimer.ResetTimer (counterAttackCooldown);
		}
	}

	public void CounterAttack(){
		anim.SetBool ("PlayCounterAttack", false);
		for (int i = 0; i < counterObjects.Count; i++) {
			Enemy15CounterAttack g = Instantiate (counterAttackPrefab, counterObjects [i].gameObject.transform.position, Quaternion.identity) as Enemy15CounterAttack;
			Vector3 dir = g.transform.position - transform.position;
			float angle = Mathf.Atan2 (dir.y, dir.x);
			g.angle = angle;
		}
	}
}
