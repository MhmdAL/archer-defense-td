using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampUpAttackspeed : MonoBehaviour
{
    public Tower Owner { get; set; }

    public float AttackSpeedPerAttack { get; set; }

    public int RampUpStackLimit { get; set; }

    private Modifier _modifier;

    private void Start()
    {
        Owner.AttackFinished += OnAfterAttack;
        Owner.CombatEnded += OnCombatEnded;
    }

    private void OnCombatEnded()
    {
        if (_modifier != null)
            _modifier.active = false;
    }

    private void OnAfterAttack()
    {        
        _modifier = new Modifier(AttackSpeedPerAttack, Name.Rapid_AtkSpdPerAttack, Type.ATTACK_SPEED, BonusOperation.Percentage);

        Owner.AddModifier(_modifier, StackOperation.Additive, RampUpStackLimit);
    }

    private void OnDestroy()
    {
        Owner.AttackFinished -= OnAfterAttack;
        Owner.CombatEnded -= OnCombatEnded;
    }
}
