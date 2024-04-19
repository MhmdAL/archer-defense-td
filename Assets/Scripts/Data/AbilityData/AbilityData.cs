using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public AbilityType Type;
    public float BaseCooldown;
}

// author the individual ability (its data)
// author which abilities are available (existence of abilities)