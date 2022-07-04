using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Path : MonoBehaviour {

	[HideInInspector]	public List<GameObject> waypoints;
	public int entrance, exit;

	void Start () {
		waypoints = new List<GameObject> ();
		for (int i = 0; i < transform.childCount; i++) {
			waypoints.Add(transform.GetChild (i).gameObject);
		}

		//Instantiate (ValueStore.sharedInstance.entranceNodePrefab, waypoints [0].transform.position, Quaternion.identity);
		//Instantiate (ValueStore.sharedInstance.exitNodePrefab, waypoints.Last().transform.position, Quaternion.identity);
	}
}
