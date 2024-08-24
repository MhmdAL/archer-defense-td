using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerAttackStrategy/ShootAtPosition")]
public class ShootAtPositionTowerAttackStrategy : TowerAttackStrategy
{
    public override void Attack(TowerAttackData data)
    {
        Shoot(data, (GameObject)Instantiate(data.Owner.bullet, LevelUtils.FarAway, Quaternion.identity), data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
    }

    public void Shoot(TowerAttackData data, GameObject projectile, float bulletDamage, float armorpen, float radius)
    {
        Vector2 startPosition = data.Owner.arrowSpawnPoint.position;
        Vector2 targetPosition = data.TargetPosition;

        Vector2 dir = targetPosition - startPosition;

        var dist = dir.magnitude;

        if (dist > data.MaxRange)
        {
            targetPosition = startPosition + dir.normalized * data.MaxRange;
        }

        Projectile bullet = projectile.GetComponentInChildren<Projectile>();

        bullet.Duration = Mathf.Sqrt(dir.magnitude) / data.Owner.bulletSpeed; // Duration of travel does not scale linearly with distance as archers are simulated to be increasing force output when targets are further away

        bullet.Owner = data.Owner;
        bullet.Damage = bulletDamage;
        bullet.ArmorPen = armorpen;
        bullet.Radius = radius;
        bullet.shotNumber = data.Owner.shotNumber;
        bullet.StartPosition = startPosition;
        bullet.TargetPosition = targetPosition;
        bullet.LingerTime = data.Owner.bulletLinger;
        bullet.HitDetectionMode = HitDetectionMode.Any; // TODO: see how we can make a good UX for target detection for manual shooting mode

        data.Owner.isInCombat = true;
        data.Owner.CombatTimer.Restart(data.Owner.CombatCooldown);
        // set cooldown back to full duration
        data.Owner.AttackCooldownTimer.Restart(data.Owner.FullCooldown);
    }
}