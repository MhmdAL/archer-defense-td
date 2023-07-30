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
}