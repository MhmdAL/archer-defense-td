using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelUtils : MonoBehaviour
{
    public static Vector2 FarAway = new Vector2(1000, 1000);

    public static List<(Path path, int waypointIdx)> GetNearestPaths(Vector3 point, float acceptableDistance = 5f)
    {
        var level = FindObjectOfType<LevelTemplate>();

        var validPaths = level.Paths
            .Select(path =>
            {
                var (distance, pt) = GetNearestDistanceToPath(point, path);

                return new
                {
                    Path = path,
                    Distance = distance,
                    Point = pt
                };
            })
            .Where(x => x.Distance <= acceptableDistance)
            .ToList();

        var result = validPaths.Select(x => (x.Path, x.Point.Value)).ToList();

        return result;
    }

    private static bool IsPathInRange(Vector3 point, float radius, Path path) => GetNearestDistanceToPath(point, path).distance <= radius;

    private static (float distance, int? point) GetNearestDistanceToPath(Vector3 point, Path path)
    {
        var shortestDistance = float.MaxValue;
        int? nearestPointIdx = null;

        for (int i = 0; i < path.PathData.Waypoints.Count; i++)
        {
            var wp = path.PathData.Waypoints[i];

            var sqrDist = (point - wp).sqrMagnitude;

            if (sqrDist < shortestDistance)
            {
                nearestPointIdx = i;
                shortestDistance = sqrDist;
            }
        }

        return nearestPointIdx == null ? (float.MaxValue, null) : (Mathf.Sqrt(shortestDistance), nearestPointIdx);
    }

    public static Vector2 CalculateAveragePathDirection(PathData p, Vector2 point)
    {
        var (curWp, index) = p.GetNearestWaypoint(point);

        Vector2? prevWp = index != 0 ? p.Waypoints[index - 1] : null;
        Vector2? nextWp = index != p.Waypoints.Count - 1 ? p.Waypoints[index + 1] : null;

        if (prevWp == null)
            Debug.LogWarning("Prev WP not found!");
        if (nextWp == null)
            Debug.LogWarning(nextWp - curWp);

        var prevToCurDir = (curWp - prevWp) ?? Vector2.zero;
        var curToNextDir = (nextWp - curWp) ?? Vector2.zero;

        return (prevToCurDir + curToNextDir).normalized;
    }

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
