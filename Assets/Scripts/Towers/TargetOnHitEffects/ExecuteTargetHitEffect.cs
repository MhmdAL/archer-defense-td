using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExecuteTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                if (target.CurrentHP < target.MaxHP.Value * 0.2f)
                {
                    target.Damage(999999, 0, DamageSource.Normal, data.Owner);
                }
            }
        }
    }
}