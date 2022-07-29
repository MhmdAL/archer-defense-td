using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExecuteTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Target != null)
        {
            if(data.Target.CurrentHP < data.Target.MaxHP.Value * 0.2f)
            {
                data.Target.Damage(999999, 0, DamageSource.Normal, data.Owner);
            }
        }
    }
}