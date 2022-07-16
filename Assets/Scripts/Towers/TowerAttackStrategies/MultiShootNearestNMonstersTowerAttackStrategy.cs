using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;

[CreateAssetMenu]
public class MultiShootNearestNMonstersTowerAttackStrategy : TowerAttackStrategy
{
    [field: SerializeField]
    public int ShotCount { get; set; } = 2;

    [field: SerializeField]
    public bool MultiShootSecondaries { get; set; } = false;

    public override void Attack(TowerAttackData data)
    {
        var targets = data.SecondaryTargets;

        var targetCount = data.PrimaryTarget != null ? 1 : 0 + targets.Count;

        if (data.PrimaryTarget != null)
        {
            ShootTarget(data, data.PrimaryTarget, true);
        }

        foreach (var target in targets)
        {
            if (target != null)
            {
                ShootTarget(data, target, MultiShootSecondaries);
            }
        }

        data.Owner.AttackCooldownTimer.ResetTimer(data.Owner.fullcooldown);

        if (targetCount > 0)
        {
            data.OnAfterAttack?.Invoke();
        }
    }

    private void ShootTarget(TowerAttackData data, Monster target, bool multiShot)
    {
        var shots = 0;

        Shoot(data, target, (GameObject)Instantiate(data.Owner.bullet, data.Owner.arrowSpawnPoint.position, Quaternion.identity), data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
        shots++;

        if (multiShot)
        {
            Timer.Register(0.25f, (timer) =>
            {
                if (target == null)
                {
                    return;
                }

                Shoot(data, target, (GameObject)Instantiate(data.Owner.bullet, data.Owner.arrowSpawnPoint.position, Quaternion.identity), data.Owner.AD.Value, data.Owner.AP.Value, data.Owner.bulletRadius);
                shots++;

                if (shots >= ShotCount)
                {
                    Timer.Cancel(timer);
                }
            }, null, true, false, data.Owner);
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
        // data.Owner.AttackCooldownTimer.ResetTimer(data.Owner.fullcooldown);
    }
}

public class TowerAttackData
{
    public Action OnAfterAttack;

    public Tower Owner { get; set; }
    public Monster PrimaryTarget { get; set; }
    public List<Monster> SecondaryTargets { get; set; }
    public List<Monster> MonstersInRange { get; set; }

}