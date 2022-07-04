using UnityEngine;
using System.Collections;

public class ExitPoint : MonoBehaviour, IAttacker {


    void OnTriggerEnter2D(Collider2D other)
    {
		Monster m = other.GetComponent<Monster> ();
		if (m != null) {
			m.Damage (1000000, 1000000, DamageSource.Exit, this);
		}
    }
}
