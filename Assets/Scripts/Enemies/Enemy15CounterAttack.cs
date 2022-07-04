using UnityEngine;
using System.Collections;

public class Enemy15CounterAttack : MonoBehaviour {

	public float speed;
	public float delayTillSelfDestruct;
	public float radius;

	[HideInInspector]	public Vector3 target;
	[HideInInspector]	public float angle;
	float progress = 0;

	void Start(){
		target = new Vector3 (radius * Mathf.Cos(angle) + transform.position.x,
			radius * Mathf.Sin(angle) + transform.position.y, transform.position.z);
	}

	void Update () {
		float step = (speed * Time.deltaTime) / Vector3.Distance(transform.position, target);
		progress += step;
		transform.position = Vector3.Lerp (transform.position, target, Mathf.Clamp(progress, 0, 1));
		if (progress >= 1) {
			Destroy (gameObject, delayTillSelfDestruct);
		}
	}
		
	void OnTriggerEnter2D(Collider2D c){
		if (c.GetComponent<Projectile> () != null || c.GetComponent<ArtilleryArrow>() != null) {
			Destroy (c.gameObject);
			Destroy (gameObject);
		}
	}
}
