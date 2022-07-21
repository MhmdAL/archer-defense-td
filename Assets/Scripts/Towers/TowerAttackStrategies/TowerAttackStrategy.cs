using UnityEngine;

/// <summary>
/// A strategy which defines how the tower shoots its targets.
/// </summary>
public abstract class TowerAttackStrategy : ScriptableObject
{
    public abstract void Attack(TowerAttackData data);
}