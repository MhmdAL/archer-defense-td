using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEffect/SlowOnHit")]
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