using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[CreateAssetMenu]
public class HeadshotTargetHitEffect : TargetHitEffect
{
    public override void OnTargetHit(AttackData data)
    {
        if (data.Targets != null)
        {
            foreach (var target in data.Targets)
            {
                // var headShotTimer = data.Owner.ExtraData["HeadshotTimer"] as Timer;

                // if (headShotTimer.GetTimeRemaining() <= 0)
                // {
                //     target.Damage(data.Projectile.Damage * 5, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);

                //     headShotTimer.Restart();
                // }
                // else
                // {
                //     target.Damage(data.Projectile.Damage, data.Projectile.ArmorPen, DamageSource.Normal, data.Owner);
                // }
            }
        }
    }
}