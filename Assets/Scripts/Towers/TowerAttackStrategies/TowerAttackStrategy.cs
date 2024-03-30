using UnityEngine;

/// <summary>
/// A strategy which defines how the tower shoots its targets.
/// Basically what actions will be taken when the tower is ready to shoot. Will it fire to a single target? Multiple? Will it fire to mouse position? etc.
/// </summary>
public abstract class TowerAttackStrategy : ScriptableObject, ITowerAttackStrategy
{
    public abstract void Attack(TowerAttackData data);
}

public interface ITowerAttackStrategy
{
    
}