using UnityEngine;

public abstract class PotionEffect : ScriptableObject 
{
    public abstract void Apply(Tower target);
}