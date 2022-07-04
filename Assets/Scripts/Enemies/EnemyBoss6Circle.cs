using UnityEngine;
using System.Collections;

public class EnemyBoss6Circle : MonoBehaviour {


	void Update () {
		transform.Rotate (Vector3.forward * Time.deltaTime * 30);
	}

	void OnTriggerEnter2D(Collider2D c){
		if (c.GetComponent<Projectile> () != null || c.GetComponent<ArtilleryArrow>() != null) {
			Destroy (c.gameObject);
		}
	}
}