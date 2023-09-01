using System.Collections.Generic;
using UnityEngine;

public interface IProjectileTarget
{
    void OnProjectileHit(Projectile p, Vector2 hitpoint);
}