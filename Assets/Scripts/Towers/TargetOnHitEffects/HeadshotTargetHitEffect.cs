using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[CreateAssetMenu]
public class HeadshotTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(TargetHitData data)
    {
        if (data.Target != null)
        {
            var headShotTimer = data.Owner.ExtraData["HeadshotTimer"] as Timer;
            
            if (headShotTimer.GetTimeRemaining() <= 0)
            {
                data.Target.Damage(data.Projectile.Damage * 5, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);

                headShotTimer.Restart();
            }
            else
            {
                data.Target.Damage(data.Projectile.Damage, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);
            }
        }
    }
}