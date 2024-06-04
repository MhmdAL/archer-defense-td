using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColliderExtensions
{
    public static bool ContainsPoint<T>(this IEnumerable<T> array, Vector2 point) where T : Collider2D
    {
        return array.Any(x => x.OverlapPoint(point));
    }
}