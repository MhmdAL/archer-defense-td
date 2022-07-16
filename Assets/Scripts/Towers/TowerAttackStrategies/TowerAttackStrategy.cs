using UnityEngine;

public abstract class TowerAttackStrategy : ScriptableObject
{
    public abstract void Attack(TowerAttackData data);
}