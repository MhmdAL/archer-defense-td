using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyBoss1Data : EnemyData
{
    [field: SerializeField]
    public float AttackSpeedDebuffDuration { get; set; }

    [field: SerializeField]
    public float AttackSpeedDebuffValue { get; set; }
}
