using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    [field: Multiline]
    public string Description { get; set; }

    [field: SerializeField]
    public int MaxHealth { get; set; }

    [field: SerializeField]
    public float Movespeed { get; set; }

    [field: SerializeField]
    public int SilverValue { get; set; }

    [field: SerializeField]
    public int LivesValue { get; set; }
}
