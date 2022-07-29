using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityTimer;
using System.Linq;
using System;

[System.Serializable]
public class Stat
{
    public Stat(Type t)
    {
        type = t;
        _modifiers = new List<ModifierV2>();
    }

    public void Modify(float value, BonusOperation bo, string name)
    {
        Modify(value, bo, name, null, null, null);
    }

    public void Modify(float value, BonusOperation bo, string name, float? duration = null, int? stackLimit = null,
        Action onModifierElapsed = null)
    {
        var existingModifier = _modifiers.FirstOrDefault(x => x.ModifierName == name);

        if (existingModifier is not null)
        {
            if (existingModifier.StackLimit == 0 || existingModifier.StackLimit > existingModifier.CurrentStacks)
            {
                existingModifier.CurrentStacks++;
            }

            existingModifier.Timer?.Restart(duration);
        }
        else
        {
            var newModifier = new ModifierV2(value, bo, name, stackLimit);

            newModifier.CurrentStacks = 1;

            if (duration != null)
            {
                newModifier.Timer = Timer.Register(duration.Value, (t) =>
                {
                    _modifiers.Remove(newModifier);

                    onModifierElapsed?.Invoke();
                });
            }

            _modifiers.Add(newModifier);
        }
    }

    [field: SerializeField]
    public float BaseValue { get; set; }

    public float BonusValue
    {
        get
        {
            var flatModifiers = _modifiers.Where(x => x.BonusOperation == BonusOperation.Flat);
            var flatBonus = flatModifiers.Sum(x => x.Value * x.CurrentStacks);

            var percentageModifiers = _modifiers.Where(x => x.BonusOperation == BonusOperation.Percentage);
            var percentageBonus = percentageModifiers.Sum(x => x.Value * x.CurrentStacks);

            var percentageBonusValue = (BaseValue + flatBonus) * percentageBonus;

            return flatBonus + percentageBonusValue;
        }
    }

    private List<ModifierV2> _modifiers;

    [System.NonSerialized] public Type type;

    [System.NonSerialized] public bool locked = false;

    [System.NonSerialized] public float multiplier = 0;
    [System.NonSerialized] public float overallMultiplier = 1;
    [System.NonSerialized] public float flatBonus = 0;
    float totalValue;

    public float Value
    {
        get
        {
            if (!locked)
            {
                totalValue = BaseValue + BonusValue;
                return totalValue * overallMultiplier;
            }
            else
            {
                return totalValue * overallMultiplier;
            }
        }
        set
        {
            if (!locked)
            {
                totalValue = value;
            }
        }
    }
}
