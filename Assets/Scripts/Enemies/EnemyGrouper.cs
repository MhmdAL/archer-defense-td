using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrouper : Monster
{
    public new EnemyGrouperData EnemyData => base.EnemyData as EnemyGrouperData;

    protected override void Update()
    {
        base.Update();

        var coliders = Physics2D.OverlapCircleAll(transform.position, EnemyData.GroupBuffRadius);

        var isGrouped = false;

        foreach (var col in coliders)
        {
            if (col.TryGetComponent<EnemyGrouper>(out var _))
            {
                isGrouped = true;
                break;
            }
        }

        if (isGrouped)
        {
            MaxHP.Modify(EnemyData.GroupBuffHealthPercentage, BonusOperation.Percentage, "GroupBuff", 0.25f, 1);
            CurrentHP += MaxHP.BaseValue * (1 + EnemyData.GroupBuffHealthPercentage);
        }
    }
}
