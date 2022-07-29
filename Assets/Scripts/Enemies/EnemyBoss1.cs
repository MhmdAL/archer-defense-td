﻿using UnityEngine;
using System.Collections;

public class EnemyBoss1 : Monster
{
    public new EnemyBoss1Data EnemyData => base.EnemyData as EnemyBoss1Data;

    public override void Damage(float damage, float armorpen, DamageSource source, IAttacker killer)
    {
        base.Damage(damage, armorpen, source, killer);
        Tower t = killer as Tower;
        if (t != null)
        {
            t.AS.Modify(EnemyData.AttackSpeedDebuffValue, BonusOperation.Percentage, Name.EnemyBoss1_AtkSlow.ToString(),
             EnemyData.AttackSpeedDebuffDuration, 1);
        }
    }
}
