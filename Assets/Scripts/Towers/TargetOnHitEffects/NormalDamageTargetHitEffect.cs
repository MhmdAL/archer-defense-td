using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NormalDamageTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(AttackData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                if (target is Unit unit)
                {
                    unit.Damage(data.Damage, data.ArmorPen, DamageSource.Normal, data.Owner, new DamageMetaData { Projectile = data.Projectile });
                }
            }
        }
    }
}

public class AttackData
{
    public IAttacking Owner { get; set; }
    public Vector3 HitPosition { get; set; }
    public float HitRadius { get; set; }
    public List<IProjectileTarget> Targets { get; set; }

    public float Damage { get; set; }
    public float ArmorPen { get; set; }
    public Projectile Projectile { get; set; }
}