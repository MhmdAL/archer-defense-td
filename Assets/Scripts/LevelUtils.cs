using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUtils : MonoBehaviour
{
    public static Vector2 FarAway = new Vector2(1000, 1000);

    public static Vector3 GetNearestWaypoint(Vector3 point)
    {
        var level = FindObjectOfType<LevelTemplate>();

        var path = FindNearestPath(point, level.Paths);

        var reversedPath = path.PathData.ReversePath();

        point.z = 0;
        return reversedPath.GetNearestWaypoint(point).Item1;
    }

    public static Path FindNearestPath(Vector3 point)
    {
        var level = FindObjectOfType<LevelTemplate>();

        return FindNearestPath(point, level.Paths);
    }

    public static Path FindNearestPath(Vector3 point, List<Path> paths)
    {
        Path nearestPath = null;
        float shortestDistance = float.MaxValue;

        foreach (var path in paths)
        {
            foreach (var waypoint in path.PathData.Waypoints)
            {
                float distance = Vector3.Distance(point, waypoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPath = path;
                }
            }
        }

        return nearestPath;
    }
}
