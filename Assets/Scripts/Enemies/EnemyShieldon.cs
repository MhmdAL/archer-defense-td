using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldon : Monster
{
    public new EnemyShieldonData EnemyData => base.EnemyData as EnemyShieldonData;

    protected override void Update()
    {
        base.Update();

        if (!InCombat)
        {
            var shieldGain = EnemyData.ShieldGainRate * Time.deltaTime;
            CurrentShield = Mathf.Clamp(CurrentShield + shieldGain, 0, EnemyData.MaxShieldHp);
        }
    }
}
