using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Target is Monster m)
        {
            m.Movespeed.Modify(-0.9f, BonusOperation.Percentage, "SlowOnHit", 1, 1);
        }
    }
}