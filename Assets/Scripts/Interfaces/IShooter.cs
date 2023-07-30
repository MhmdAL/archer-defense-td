using System.Collections.Generic;

public interface IShooter
{
    void OnTargetHit(List<Unit> unitsHit, Projectile p, int shotNumber);
}