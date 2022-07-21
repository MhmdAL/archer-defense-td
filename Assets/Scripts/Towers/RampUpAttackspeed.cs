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
    }

    private void OnAfterAttack()
    {
        Owner.AR.Modify(AttackSpeedPerAttack, BonusOperation.Percentage, BuffNames.RAMP_UP_ATK_SPD, 2f, RampUpStackLimit);
    }

    private void OnDestroy()
    {
        Owner.AttackFinished -= OnAfterAttack;
        Owner.CombatEnded -= OnCombatEnded;
    }
}
