using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> array)
    {
        System.Random random = new System.Random();
        return array.OrderBy(x => random.Next()).AsEnumerable();
    }
}