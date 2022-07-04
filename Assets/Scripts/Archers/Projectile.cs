using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	[HideInInspector]
	public Tower ownerTower;

	[HideInInspector]
	public Monster targetMonster;

	[HideInInspector]
	public string currentProjectile;

	[HideInInspector]
    public float damage;

	[HideInInspector]
    public float radius;

	[HideInInspector]
    public float armorPen;

	[HideInInspector]
	public float progress;

	[HideInInspector]
	public float speed;

	[HideInInspector]
	public int shotNumber;

	[HideInInspector]
	public bool isAboutToKill;

	private MonsterManager mm;

	private Vector3 startPos, endPos, dir;

	private float pathLength, step;

	private Transform myTransform;
	private Transform targetTransform;


	void Start(){
		mm = ValueStore.sharedInstance.monsterManagerInstance;
		startPos = ownerTower.transform.position;

		myTransform = transform;

		targetTransform = targetMonster.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(ownerTower == null || targetTransform == null)
        {
			Destroy (transform.root.gameObject);
			return;
        }
		if (targetTransform != null && ownerTower != null) {
			dir = targetTransform.position - transform.position;

			float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle + 90, Vector3.forward);

			endPos = targetTransform.transform.position;

			pathLength = Vector3.Distance (startPos, endPos);
			step = speed / pathLength;

			progress += step;

			myTransform.position = Vector3.Lerp (startPos, endPos, progress);

			if (transform.position == endPos) {
				if (radius > 0) {
					List<Monster> aoeTargets = new List<Monster> ();
					Collider2D[] cols = Physics2D.OverlapCircleAll (myTransform.position, radius);
					foreach (Collider2D c in cols) {
						Monster mo = c.GetComponent<Monster> ();
						if (mo != null && mo != targetMonster) {
							aoeTargets.Add (mo);
						}
					}
					if (ownerTower != null) {
						ownerTower.BulletHit (targetMonster, this, aoeTargets, shotNumber);		
						BulletHit (targetMonster, aoeTargets);
					}
				} else {
					if (ownerTower != null) {
						ownerTower.BulletHit (targetMonster, this, null, shotNumber);		
						BulletHit (targetMonster);
					} else {
						targetMonster.Damage (damage, 0, DamageSource.Normal, ownerTower);
					}
				}
			}
		}
    }

	public void BulletHit(Monster m, List<Monster> aoeTargets = null)
    {
		Destroy (transform.root.gameObject);
    }
}
