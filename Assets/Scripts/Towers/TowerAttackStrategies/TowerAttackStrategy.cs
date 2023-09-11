using UnityEngine;

/// <summary>
/// A strategy which defines how the tower shoots its targets.
/// </summary>
public abstract class TowerAttackStrategy : ScriptableObject, ITowerAttackStrategy
{
    public abstract void Attack(TowerAttackData data);
}

public interface ITowerAttackStrategy
{
    
}