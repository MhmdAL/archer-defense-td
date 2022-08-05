using UnityEngine;

public class EnemyDeflector : Monster
{
    public override void OnProjectileHit(Projectile p, Vector2 hitpoint)
    {
        var targetDir = (CurrentPath.waypoints[CurrentWaypoint + 1].transform.position - transform.position).normalized;

        if (targetDir.x > 0 && p.transform.position.x > transform.position.x + 2)
        {
            p.Reset(hitpoint, transform.position + Vector3.right * 8 + Vector3.down * 2f * Mathf.Sign(p.transform.position.y - transform.position.y));
        }
        else if (targetDir.x <= 0 && p.transform.position.x <= transform.position.x - 2)
        {
            p.Reset(hitpoint, transform.position + Vector3.left * 8);
        }
        else
        {
            base.OnProjectileHit(p, hitpoint);
        }
    }
}
