using UnityEngine;
using System.Collections;

public class EnemyBoss1 : Monster
{
    public new EnemyBoss1Data EnemyData => base.EnemyData as EnemyBoss1Data;

    public override void Damage(float damage, float armorpen, DamageSource source, IAttacking killer, DamageMetaData damageMeta)
    {
        base.Damage(damage, armorpen, source, killer, damageMeta);
        Tower t = killer as Tower;
        if (t != null)
        {
            t.AS.Modify(EnemyData.AttackSpeedDebuffValue, BonusType.Percentage, Name.EnemyBoss1_AtkSlow.ToString(),
             EnemyData.AttackSpeedDebuffDuration, 1);
        }
    }
}
