using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NormalDamageTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Target != null)
        {
            data.Target.Damage(data.Projectile.Damage, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);
        }
    }
}

public class TargetHitData
{
    public Tower Owner { get; set; }
    public Unit Target { get; set; }
    public Projectile Projectile { get; set; }
}