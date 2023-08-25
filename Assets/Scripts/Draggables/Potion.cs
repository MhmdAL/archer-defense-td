using System.Collections.Generic;
using UnityEngine;

public class Potion : Draggable
{
    public List<PotionEffect> PotionEffects;

    protected override void Apply(Tower target)
    {
        base.Apply(target);

        foreach (var effect in PotionEffects)
        {
            effect.Apply(target);
        }
    }
}

