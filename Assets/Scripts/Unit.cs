using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public abstract class Unit : MonoBehaviour
{
    public event Action HealthChanged;
    public event Action<Unit, float, IAttacker> Damaged;

    public Stat MaxHP { get; set; }
    public Stat Armor { get; set; }
    public Stat DamageModifier { get; set; }

    public bool InCombat { get; set; }
    public float CombatTimer { get; set; } = 2f;

    private float _currentHP;
    public float CurrentHP
    {
        get
        {
            return _currentHP;
        }
        set
        {
            _currentHP = value;
            HealthChanged?.Invoke();
        }
    }

    private float _currentShield;
    public float CurrentShield
    {
        get
        {
            return _currentShield;
        }
        set
        {
            _currentShield = value;
            HealthChanged?.Invoke();
        }
    }

    public bool IsDead { get; set; }

    protected Timer _combatTimer;

    protected virtual void Awake()
    {
        _combatTimer = Timer.Register(CombatTimer, OnCombatTimerElapsed, isDoneWhenElapsed: false);

        MaxHP = new Stat(Type.Health);
    }

    protected virtual void Start()
    {
        CurrentHP = MaxHP.Value;
    }

    public virtual void Damage(float damage, float armorpen, DamageSource source, IAttacker killer)
    {
        InCombat = true;
        _combatTimer.Restart(CombatTimer);

        float modifiedDamage = damage * DamageModifier.Value;
        float finalDamage = 0;
        if (this != null)
        {
            if (CurrentHP + CurrentShield > 0)
            {
                armorpen = Mathf.Clamp(armorpen, 0, 1);
                finalDamage = (float)(modifiedDamage * (50 / (50 + (Armor.Value * (1 - armorpen)))));

                var newShieldValue = CurrentShield - finalDamage;

                if (newShieldValue < 0)
                {
                    CurrentHP += newShieldValue;
                    CurrentShield = 0;
                }
                else
                {
                    CurrentShield = newShieldValue;
                }
            }

            Damaged?.Invoke(this, finalDamage, killer);

            if (CurrentHP <= 0)
            {
                OnDeath(source, killer);
            }
        }
    }

    protected virtual void OnDeath(DamageSource source, IAttacker killer)
    {
        if (!IsDead)
        {
            Destroy(transform.root.gameObject);

            IsDead = true;
        }
    }

    private void OnCombatTimerElapsed(Timer t)
    {
        InCombat = false;
    }
}
