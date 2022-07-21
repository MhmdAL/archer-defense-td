using System;
using UnityTimer;

public class ModifierV2
{
    public ModifierV2(float value, BonusOperation op, string name, int? stackLimit = null)
    {
        this.Value = value;
        this.BonusOperation = op;
        this.ModifierName = name;

        this.StackLimit = stackLimit ?? 0;
    }

    public float Value { get; set; }

    public BonusOperation BonusOperation { get; set; }

    public string ModifierName { get; set; }

    public int StackLimit { get; set; }

    public int CurrentStacks { get; set; }

    public Timer Timer { get; set; }
}