using System.Collections.Generic;
using UnityEngine;

public interface IShooter
{
    void OnTargetHit(Vector3 TargetPosition, List<Unit> unitsHit, Projectile p, int shotNumber);
}