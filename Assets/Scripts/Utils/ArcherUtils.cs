using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUtils
{
    public static float CalculateProjectileDuration(Vector2 shootDir, float bulletSpeed)
    {
        return 0.5f + Mathf.Sqrt(shootDir.magnitude) / bulletSpeed;
    }

    public static Vector2 CalculateProjectilePrediction(Vector2 shootDir, Vector2 targetVelocity, float bulletDuration)
    {
        var basePrediction = targetVelocity.normalized * 1;

        var dot = Mathf.Abs(Vector3.Dot(targetVelocity.normalized, shootDir.normalized));

        return basePrediction * (1 - dot) + targetVelocity * dot * bulletDuration;
    }
}
