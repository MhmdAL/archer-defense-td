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
                target.Damage(data.Damage, data.ArmorPen, DamageSource.Normal, data.Owner);
            }
        }
    }
}

public class TargetHitData
{
    public IShooter Owner { get; set; }
    public Vector3 HitPosition { get; set; }
    public float HitRadius { get; set; }
    public List<Unit> Targets { get; set; }

    public float Damage { get; set; }
    public float ArmorPen { get; set; }
    public Projectile Projectile { get; set; }
}