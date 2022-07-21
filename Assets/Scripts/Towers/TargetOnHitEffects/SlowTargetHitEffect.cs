using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        data.Target.MS.Modify(-0.9f, BonusOperation.Percentage, "SlowOnHit", 1, 1);
    }
}