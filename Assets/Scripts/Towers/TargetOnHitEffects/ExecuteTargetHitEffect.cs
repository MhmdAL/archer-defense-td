using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEffect/Execute")]
public class ExecuteTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(AttackData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                if (target is Unit unit && unit.CurrentHP < unit.MaxHP.Value * 0.2f)
                {
                    unit.Damage(999999, 0, DamageSource.Normal, data.Owner, new DamageMetaData { Projectile = data.Projectile });
                }
            }
        }
    }
}