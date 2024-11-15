﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitPoint : MonoBehaviour, IAttacking
{
	public List<TargetHitEffect> OnHitEffects => new List<TargetHitEffect>();

	void OnTriggerEnter2D(Collider2D other)
	{
		Monster m = other.GetComponent<Monster>();
		if (m != null)
		{
			m.Damage(1000000, 1000000, DamageSource.Exit, this, null);
		}
	}
}
