using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ShootNearestNMonstersTowerAttackStrategy : TowerAttackStrategy
{
    public override void Attack(TowerAttackData data)
    {
        ShootNearestNMonsters(data);
    }

    public void ShootNearestNMonsters(TowerAttackData data)
    {
        var targets = data.SecondaryTargets;

        var targetCount = 0;

        if (data.PrimaryTarget is not null)
        {
            Shoot(data, data.PrimaryTarget, null, data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
            targetCount++;
        }

        foreach (var target in targets)
        {
            if (target is not null)
            {
                Shoot(data, target, null, data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
                targetCount++;
            }
        }

        data.Owner.AttackCooldownTimer.Restart(data.Owner.FullCooldown);

        if (targetCount > 0)
        {
            data.OnAfterAttack?.Invoke();
        }
    }

    public void Shoot(TowerAttackData data, Monster target, GameObject projectile, float bulletDamage, float armorpen, float radius)
    {
        var targetCollider = target.GetComponentInChildren<Collider2D>();

        var (bullet, dir) = Projectile.Fire(new ProjectileSpawnData(data.Owner, data.Owner.bullet, data.Owner.arrowSpawnPoint.position, targetCollider.bounds.center)
        {
            Owner = data.Owner,
            Damage = bulletDamage,
            ArmorPen = armorpen,
            Radius = radius,
            SpawnPosition = data.Owner.arrowSpawnPoint.position,
            LingerTime = data.Owner.bulletLinger
        });

        Vector2 targetPosition = targetCollider.bounds.center;

        bullet.Duration = ArcherUtils.CalculateProjectileDuration(dir, data.Owner.bulletSpeed);

        targetPosition += ArcherUtils.CalculateProjectilePrediction(dir, target.movementTracker.CurrentVelocity, bullet.Duration);

        bullet.TargetPosition = targetPosition;
        bullet.shotNumber = data.Owner.shotNumber;

        data.Owner.isInCombat = true;
        data.Owner.CombatTimer.Restart(data.Owner.CombatCooldown); // set cooldown back to full duration
        data.Owner.AttackCooldownTimer.Restart(data.Owner.FullCooldown); // set cooldown back to full duration
        
        if (target != null)
        {
            if (MonsterManager.DoesKill(target, bulletDamage, armorpen))
            {
                bullet.isAboutToKill = true;
            }
        }
    }
}