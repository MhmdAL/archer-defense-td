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

        data.Owner.AttackCooldownTimer.Restart(data.Owner.FullCooldown);

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
            if (ValueStore.Instance.monsterManagerInstance.DoesKill(target, bulletDamage, armorpen))
            {
                bullet.isAboutToKill = true;
            }
        }

        var targetDir = (target.CurrentPath.waypoints[target.CurrentWaypoint + 1].transform.position - target.transform.position).normalized;

        var randomOffset = UnityEngine.Random.Range(-1, 1f);
        var targetPosition = target.transform.position + targetDir * (data.Owner.bulletSpeed * 0.9f * target.Movespeed.Value + randomOffset);

        //Instantiate (archerShotParticle, arrowSpawnPoint.position, Quaternion.identity);
        bullet.Owner = data.Owner;
        bullet.Damage = bulletDamage;
        bullet.ArmorPen = armorpen;
        bullet.Radius = radius;
        bullet.shotNumber = data.Owner.shotNumber;
        bullet.StartPosition = data.Owner.transform.position;
        bullet.TargetPosition = targetPosition;
        bullet.Duration = data.Owner.bulletSpeed;

        data.Owner.isInCombat = true;
        data.Owner.CombatTimer.Restart(data.Owner.CombatCooldown);
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