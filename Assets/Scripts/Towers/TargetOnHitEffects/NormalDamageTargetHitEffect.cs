using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NormalDamageTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                target.Damage(data.Projectile.Damage, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);
            }
        }
    }
}

public class TargetHitData
{
    public Tower Owner { get; set; }
    public Vector3 HitPosition { get; set; }
    public float HitRadius { get; set; }
    public List<Unit> Targets { get; set; }
    public Projectile Projectile { get; set; }
}