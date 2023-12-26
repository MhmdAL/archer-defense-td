using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Monster))]
public class MovespeedByHealth : MonoBehaviour
{
    public float minimumHealthPercentage = 0.3f;

    private Monster _target;

    private void Awake()
    {
        _target = GetComponent<Monster>();

        _target.HealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged()
    {
        var speedScale = Mathf.Clamp(_target.CurrentHP / _target.MaxHP.Value, minimumHealthPercentage, 1f) - 1;

        _target.MoveSpeed.Modify(speedScale, BonusType.Percentage, nameof(MovespeedByHealth), stackLimit: 1);
    }
}