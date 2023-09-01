using System.Collections.Generic;
using UnityEngine;

public interface IShooter : IAttacker
{
    void OnTargetHit(Vector3 TargetPosition, List<IProjectileTarget> unitsHit, Projectile p, int shotNumber);
}