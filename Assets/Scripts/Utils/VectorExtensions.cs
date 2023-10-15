using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 ToWorldPosition(this Vector2 screenPos, Camera camera)
    {
        return camera.ScreenToWorldPoint(screenPos);
    }

    public static Vector3 ToWorldPosition(this Vector3 screenPos, Camera camera)
    {
        return camera.ScreenToWorldPoint(screenPos);
    }

    public static Vector2 ToScreenPosition(this Vector2 screenPos, Camera camera)
    {
        return camera.WorldToScreenPoint(screenPos);
    }

    public static Vector3 ToScreenPosition(this Vector3 screenPos, Camera camera)
    {
        return camera.WorldToScreenPoint(screenPos);
    }

    public static (Vector3, int) FindClosestWithIndex(this Vector3 referencePoint, List<Vector3> points)
    {
        if (points == null || points.Count == 0)
        {
            throw new System.ArgumentException("The list cannot be null or empty.");
        }

        Vector3 closestPoint = points[0];
        int closestIndex = 0;
        float closestDistanceSqr = Vector3.SqrMagnitude(closestPoint - referencePoint);

        for (int i = 0; i < points.Count; i++)
        {
            float currentDistanceSqr = Vector3.SqrMagnitude(points[i] - referencePoint);
            if (currentDistanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = currentDistanceSqr;
                closestPoint = points[i];
                closestIndex = i;
            }
        }

        return (closestPoint, closestIndex);
    }
}