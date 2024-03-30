using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tower")]
public class TowerData : ScriptableObject
{
    public float BaseAttackDamage = 10f;
    public float BaseAttackRange = 16f;
    public float ManualAttackRangeMultiplier = 1.5f;
    public float BaseAttackSpeed = .5f;
    public float BaseMoveSpeed = 75f;
}
