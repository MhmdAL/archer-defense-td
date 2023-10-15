using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SlowTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(AttackData data)
    {
        foreach (var target in data.Targets)
        {
            if (target is IMoving m)
            {
                m.MoveSpeed.Modify(-0.9f, BonusType.Percentage, "SlowOnHit", 1, 1);
            }
        }
    }
}