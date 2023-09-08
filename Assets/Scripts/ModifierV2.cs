using System;
using UnityTimer;

public class ModifierV2
{
    public ModifierV2(float value, BonusType op, string name, int stackLimit = 0)
    {
        this.Value = value;
        this.BonusOperation = op;
        this.ModifierName = name;

        this.StackLimit = stackLimit;
    }

    public Action ModifierEnded;

    public float Value { get; set; }

    public BonusType BonusOperation { get; set; }

    public string ModifierName { get; set; }

    public int StackLimit { get; set; }

    public int CurrentStacks { get; set; }

    public Timer Timer { get; set; }
}

public enum BonusType
{
	Flat,
	Percentage,
	OverallMultiplier
}