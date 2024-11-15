using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUtils
{
    public static float CalculateProjectileDuration(Vector2 shootDir, float bulletSpeed)
    {
        return 0.5f + Mathf.Sqrt(shootDir.magnitude) / bulletSpeed;
    }

    public static Vector2 CalculateProjectilePredictionOffset(Vector2 shootDir, Vector2 targetVelocity, float bulletDuration)
    {
        var basePrediction = targetVelocity.normalized * 1;

        var dot = Mathf.Clamp(Mathf.Abs(Vector3.Dot(targetVelocity.normalized, shootDir.normalized)), 0.75f, 1) * 1.5f;

        return basePrediction * (1 - dot) + targetVelocity * dot * bulletDuration;
    }

    public static Vector2 CalculateProjectilePredictionOffset(Vector2 archerPosition, Vector2 enemyPosition, Vector2 enemyVelocity, float projectileSpeed)
    {
        return enemyPosition + enemyVelocity * projectileSpeed;
    }

}
