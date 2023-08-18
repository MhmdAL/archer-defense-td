using System.Collections.Generic;
using UnityEngine;

public interface IShooter : IAttacker
{
    List<TargetHitEffect> OnHitEffects { get; }
    void OnTargetHit(Vector3 TargetPosition, List<Unit> unitsHit, Projectile p, int shotNumber);
}