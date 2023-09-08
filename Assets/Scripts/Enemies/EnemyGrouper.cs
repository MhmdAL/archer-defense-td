using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGrouper : Monster
{
    public new EnemyGrouperData EnemyData => base.EnemyData as EnemyGrouperData;

    private bool _isGrouped;

    protected override void Update()
    {
        base.Update();

        var coliders = Physics2D.OverlapCircleAll(transform.position, EnemyData.GroupBuffRadius);

        var nearbyGroupers = new List<EnemyGrouper>();

        foreach (var col in coliders)
        {
            if (col.TryGetComponent<EnemyGrouper>(out var g) && g != this)
            {
                nearbyGroupers.Add(g);
            }
        }

        if (nearbyGroupers.Any())
        {
            MaxHP.Modify(EnemyData.GroupBuffHealthPercentage * nearbyGroupers.Count, BonusType.Percentage, "GroupHPBuff", 0.2f, 1);
            MoveSpeed.Modify(EnemyData.GroupBuffMSPercentage, BonusType.Percentage, "GroupMSBuff", 0.2f, 1);

            if (_isGrouped == false) // just grouped
            {
                _isGrouped = true;
                anim.SetTrigger("groupup");
            }
        }
        else
        {
            _isGrouped = false;
        }
    }
}
