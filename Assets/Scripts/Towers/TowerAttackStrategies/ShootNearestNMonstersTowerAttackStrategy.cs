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
            Shoot(data, data.PrimaryTarget, (GameObject)Instantiate(data.Owner.bullet, data.Owner.arrowSpawnPoint.position, Quaternion.identity), data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
            targetCount++;
        }

        foreach (var target in targets)
        {
            if (target is not null)
            {
                Shoot(data, target, (GameObject)Instantiate(data.Owner.bullet, data.Owner.arrowSpawnPoint.position, Quaternion.identity), data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
                targetCount++;
            }
        }

        if (targetCount > 0)
        {
            data.OnAfterAttack?.Invoke();
        }
    }

    public void Shoot(TowerAttackData data, Monster target, GameObject projectile, float bulletDamage, float armorpen, float radius)
    {
        Vector3 dir = target.transform.position - projectile.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        Projectile bullet = projectile.GetComponentInChildren<Projectile>();


        var targetPosition = target.transform.position;

        var normalizedDir = dir.normalized;

        var dot = Vector3.Dot(target.movementTracker.CurrentVelocity.normalized, normalizedDir);

        if (dot > 0)
        {
            targetPosition += target.movementTracker.CurrentVelocity.normalized * dot * bullet.Duration;
        }

        //Instantiate (archerShotParticle, arrowSpawnPoint.position, Quaternion.identity);
        bullet.Owner = data.Owner;
        bullet.Damage = bulletDamage;
        bullet.ArmorPen = armorpen;
        bullet.Radius = radius;
        bullet.shotNumber = data.Owner.shotNumber;
        bullet.StartPosition = data.Owner.transform.position;
        bullet.TargetPosition = targetPosition;
        bullet.Duration = data.Owner.bulletSpeed;
        bullet.LingerTime = data.Owner.bulletLinger;

        data.Owner.isInCombat = true;
        data.Owner.CombatTimer.Restart(data.Owner.CombatCooldown);
        // set cooldown back to full duration
        data.Owner.AttackCooldownTimer.Restart(data.Owner.FullCooldown);

        data.Owner.PlayShotSFX();

        if (target != null)
        {
            if (ValueStore.Instance.monsterManagerInstance.DoesKill(target, bulletDamage, armorpen))
            {
                bullet.isAboutToKill = true;
            }
        }
    }
}