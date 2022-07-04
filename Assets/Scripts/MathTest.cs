using UnityEngine;
using System.Collections;

public class MathTest : MonoBehaviour {

	Vector3 pos;
	
	void Start() {
		pos = transform.position;	
	}

	void Update () {
		if (Input.GetKey(KeyCode.Comma)) {
			pos.x = transform.position.x + 0.01f;
			pos.y = 0.1f * Mathf.Sin (10 / (transform.position.x + 0.01f));
			transform.position = pos;
		}
	}
}
