using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ApplyPoisonTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        data.Target.AddModifier(new Modifier(-0.9f, Name.Utility_Slow, Type.MOVEMENT_SPEED, BonusOperation.Percentage,
            ValueStore.sharedInstance.timerManagerInstance.StartTimer(1f)), StackOperation.HighestValue, 1);
    }
}