using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Path : MonoBehaviour
{

	public int entrance, exit;

	public PathData PathData { get; private set; }

	private void Start()
	{
		var waypoints = new List<GameObject>();
		for (int i = 0; i < transform.childCount; i++)
		{
			waypoints.Add(transform.GetChild(i).gameObject);
		}

		PathData = new PathData
		{
			Waypoints = waypoints.Select(x => x.transform.position).ToList()
		};

		//Instantiate (ValueStore.sharedInstance.entranceNodePrefab, waypoints [0].transform.position, Quaternion.identity);
		//Instantiate (ValueStore.sharedInstance.exitNodePrefab, waypoints.Last().transform.position, Quaternion.identity);
	}
}

public class PathData
{
	public List<Vector3> Waypoints;

	public PathData ReversePath()
	{
		var newWaypoints = Waypoints.ToList();
		newWaypoints.Reverse();

		return new PathData
		{
			Waypoints = newWaypoints
		};
	}

	public (Vector3, int) GetNearestWaypoint(Vector3 point)
	{
		return point.FindClosestWithIndex(Waypoints);
	}
}