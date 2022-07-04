/*using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CircleMover : MonoBehaviour {

	float x;
	float y;
	float curX;
	float curY;
	public float damage;
	public float radius;
	public float power;
	public int rotations;
	float angle;
	Coroutine myRoutine; 
	Coroutine launch;
	bool canStart = true;

	// Use this for initialization
	void Start () {
		x = transform.position.x;
		y = transform.position.y;

		myRoutine = StartCoroutine(CircleMove());
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && canStart == true) {
			StopCoroutine (myRoutine);
			launch = StartCoroutine (Launch ());
		} else if (Input.GetKeyDown ("q") && canStart == true) {
			myRoutine = StartCoroutine(CircleMove());
		}
	}

	IEnumerator CircleMove()
	{
		for (int i = 0; i < rotations; i++) {
			for (float j = 0; j < 2 * Mathf.PI; j += 0.1f) {
				curX = transform.position.x;
				curY = transform.position.y;
				float nextX = radius * Mathf.Cos (j) + x;
				float nextY = radius * Mathf.Sin (j) + y;
				//angle = AngleFixer (Mathf.Asin (Mathf.Abs ((nextY - y)) / radius) * Mathf.Rad2Deg, x, nextX, y, nextY);
				Vector3 dir = new Vector3(nextX - x, nextY - y, 0);
				angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;

				transform.rotation = Quaternion.AngleAxis (angle + 180, Vector3.forward);
				transform.position = new Vector3 (nextX, nextY, 0);
				yield return new WaitForSeconds (0.05f);
			}
		}
		/*for(float i = 10*Mathf.PI; i > 0; i -= 0.1f){
			transform.localPosition = new Vector3(-i, -i*Mathf.Sin(-i), 0);
			yield return new WaitForSeconds(0.5f);
		}
	}

	IEnumerator Launch()
	{
		canStart = false;
		for (int i = 0; i < power; i++) {
			transform.position = new Vector3 (2*i * Mathf.Cos (angle * Mathf.Deg2Rad) + curX, 2*i * Mathf.Sin (angle * Mathf.Deg2Rad) + curY, 0);
			//transform.rotation = Quaternion.AngleAxis (angle + 180, Vector3.forward);
			yield return new WaitForSeconds (0.05f);
		}
		canStart = true;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 10);
		foreach (Collider2D c in cols) {
			Monster mo = c.gameObject.GetComponent<Monster> ();
			//MonsterManager.Damage (mo, damage, 0, "NormalDeath");
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.GetComponent<Monster> () != null) {
			Monster m = coll.gameObject.GetComponent<Monster> ();
			MonsterManager.Damage (m, damage, 0, "NormalDeath", null);
		}
	}

	/* check in which quadrant angle is
	public float AngleFixer(float theta, float x1, float x2, float y1, float y2)
	{
		if (x2 - x1 > 0) {
			if (y2 - y1 > 0)
				theta = theta;
			else if (y2 - y1 < 0)
				theta = 360 - theta;
			else if (y2 - y1 == 0)
				theta = 0;
		} else if (x2 - x1 < 0) {
			if (y2 - y1 > 0)
				theta = 180 - theta;
			else if (y2 - y1 < 0)
				theta = 180 + theta;
			else if (y2 - y1 == 0)
				theta = 180;
		} else if (x2 - x1 == 0) {
			if (y2 - y1 > 0)
				theta = 90;
			else if (y2 - y1 < 0)
				theta = 270;
			else if (y2 - y1 == 0)
				theta = 0;
		}
		return theta;
	}

}*/
