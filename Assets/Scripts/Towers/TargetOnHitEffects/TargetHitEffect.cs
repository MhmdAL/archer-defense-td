using System.Collections.Generic;
using UnityEngine;

public abstract class TargetHitEffect : ScriptableObject
{
    public abstract void OnTargetHit(TargetHitData data);
}