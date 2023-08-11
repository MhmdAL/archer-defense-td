using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                if (target is Monster m)
                {
                    m.Movespeed.Modify(-0.9f, BonusOperation.Percentage, "SlowOnHit", 1, 1);
                }
            }
        }
    }
}