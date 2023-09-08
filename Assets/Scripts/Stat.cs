using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityTimer;
using System.Linq;
using System;

[System.Serializable]
public class Stat
{
    public Action<float, float> ValueModified;

    private float _baseValue;

    public float BaseValue
    {
        get
        {
            return _baseValue;
        }
        set
        {
            var oldValue = Value;

            _baseValue = value;

            var newValue = Value;

            ValueModified?.Invoke(oldValue, newValue);
        }
    }

    public float BonusValue
    {
        get
        {
            var flatModifiers = _modifiers.Where(x => x.BonusOperation == BonusType.Flat);
            var flatBonus = flatModifiers.Sum(x => x.Value * x.CurrentStacks);

            var percentageModifiers = _modifiers.Where(x => x.BonusOperation == BonusType.Percentage);
            var percentageBonusPercentage = percentageModifiers.Sum(x => x.Value * x.CurrentStacks);

            var percentageBonus = BaseValue * percentageBonusPercentage;

            return flatBonus + percentageBonus;
        }
    }

    public float Value => BaseValue + BonusValue;

    [NonSerialized] public Type type;

    [NonSerialized] public bool locked = false;

    [NonSerialized] public float multiplier = 0;
    [NonSerialized] public float overallMultiplier = 1;
    [NonSerialized] public float flatBonus = 0;

    private List<ModifierV2> _modifiers;

    public Stat(Type t, float baseValue, Action<float, float>? onValueModified = null)
    {
        type = t;
        _baseValue = baseValue;
        _modifiers = new List<ModifierV2>();

        ValueModified += onValueModified;
    }

    public void Modify(float value, BonusType bo, string name)
    {
        Modify(value, bo, name, null, 0, null);
    }

    public void Modify(float value, BonusType bo, string name, float? duration = null, int stackLimit = 0,
        Action onModifierElapsed = null)
    {
        var oldValue = Value;

        var existingModifier = _modifiers.FirstOrDefault(x => x.ModifierName == name);

        if (existingModifier is not null)
        {
            if (existingModifier.StackLimit == 0 || existingModifier.StackLimit > existingModifier.CurrentStacks)
            {
                existingModifier.CurrentStacks++;
            }

            existingModifier.StackLimit = stackLimit;
            existingModifier.BonusOperation = bo;
            existingModifier.Value = value;

            existingModifier.Timer?.Restart(duration);
        }
        else
        {
            var modifier = new ModifierV2(value, bo, name, stackLimit)
            {
                CurrentStacks = 1
            };

            if (onModifierElapsed != null)
            {
                modifier.ModifierEnded += onModifierElapsed;
            }

            if (duration != null)
            {
                // TODO: this probably should be tied to the entity which has the modified stat, not the scene...

                modifier.Timer = Timer.Register(duration.Value, (t) =>
                {
                    OnModifiedElapsed(modifier);

                    onModifierElapsed?.Invoke();
                });
            }

            _modifiers.Add(modifier);
        }

        var newValue = Value;

        ValueModified?.Invoke(oldValue, newValue);
    }

    private void OnModifiedElapsed(ModifierV2 modifier)
    {
        var oldValue = Value;

        _modifiers.Remove(modifier);

        var newValue = Value;

        ValueModified?.Invoke(oldValue, newValue);
    }
}
