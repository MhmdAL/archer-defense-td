using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatyImage : MonoBehaviour {

	[HideInInspector]	public float speed;
	[HideInInspector]	public float distance;

	private bool startFloating = false;

	private Vector3 startingPos;

	private Transform myTransform;

	void Start(){
		myTransform = transform;
		startingPos = myTransform.position;
	}

	void Update () {
		if (startFloating) {
			if (myTransform.position.y < startingPos.y + distance) {
				myTransform.position += new Vector3 (0, speed * Time.deltaTime, 0);	
			} else {
				Destroy (gameObject);
			}
		}
	}

	public void StartFloating(){
		startFloating = true;
	}

	void OnDestroy(){
		if (transform.parent == null) {
			
		}
	}

}
