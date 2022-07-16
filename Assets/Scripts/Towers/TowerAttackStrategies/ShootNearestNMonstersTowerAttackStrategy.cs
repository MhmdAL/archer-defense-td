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
        if (target != null)
        {
            if (ValueStore.sharedInstance.monsterManagerInstance.DoesKill(target, bulletDamage, armorpen))
            {
                bullet.isAboutToKill = true;
            }
        }

        //Instantiate (archerShotParticle, arrowSpawnPoint.position, Quaternion.identity);
        bullet.speed = data.Owner.bulletSpeed;
        bullet.ownerTower = data.Owner;
        bullet.targetMonster = target;
        bullet.damage = bulletDamage;
        bullet.armorPen = armorpen;
        bullet.radius = radius;
        bullet.shotNumber = data.Owner.shotNumber;
        bullet.currentProjectile = projectile.ToString();
        data.Owner.isInCombat = true;
        data.Owner.CombatCooldown.ResetTimer(data.Owner.combatTimer);
        // set cooldown back to full duration
        data.Owner.AttackCooldownTimer.ResetTimer(data.Owner.fullcooldown);
    }
}