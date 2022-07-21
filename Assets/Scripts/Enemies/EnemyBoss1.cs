using UnityEngine;
using System.Collections;

public class EnemyBoss1 : Monster
{

    public float attackSpeedDebuffValue;
    public float attackSpeedDebuffDuration;

    public override void Damage(float damage, float armorpen, DamageSource source, IAttacker killer)
    {
        base.Damage(damage, armorpen, source, killer);
        Tower t = killer as Tower;
        if (t != null)
        {
            t.AS.Modify(attackSpeedDebuffValue, BonusOperation.Percentage, Name.EnemyBoss1_AtkSlow.ToString(),
			 attackSpeedDebuffDuration, 1);
        }
    }
}
