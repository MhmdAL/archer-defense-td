using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTimer;
using static UnityEngine.ParticleSystem;


public abstract class Unit : MonoBehaviour, IProjectileTarget
{
    public event Action HealthChanged;
    
    public event Action<Unit, DamageSource> OnDeath;
    public event Action<Unit, float, IAttacking> Damaged;

    public GameObject HitParticles;
    public MinMaxCurve DeathTime = 1.5f;

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

        MaxHP = new Stat(Type.Health, 1, OnMaxHealthModified);
    }

    protected virtual void Start()
    {
        CurrentHP = MaxHP.Value;
    }

    private void OnMaxHealthModified(float oldValue, float newValue)
    {
        if (newValue > oldValue)
        {
            CurrentHP += newValue - oldValue;
        }
        else
        {
            CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP.Value);
        }
    }

    public virtual void Damage(float damage, float armorpen, DamageSource source, IAttacking killer, DamageMetaData damageMeta)
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
                Die(source, killer, damageMeta);
            }
        }
    }

    protected virtual void Die(DamageSource source, IAttacking killer, DamageMetaData damageMeta)
    {
        if (!IsDead)
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>();

            var deathTime = UnityEngine.Random.Range(DeathTime.constantMin, DeathTime.constantMax);

            this.AttachTimer(deathTime, x => Destroy(transform.root.gameObject), d =>
            {
                var alpha = 1 - (d / deathTime);

                sprites.ToList().ForEach(s =>
                {
                    var color = s.color;
                    color.a = alpha;
                    s.color = color;
                });
            });

            GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = false);

            OnDeath?.Invoke(this, source);

            // var pool = Instantiate(HitParticles, transform.position, Quaternion.identity);

            // var rand = UnityEngine.Random.Range(1, 1.25f);
            // pool.transform.localScale = pool.transform.localScale * rand;

            IsDead = true;
        }
    }

    private void OnCombatTimerElapsed(Timer t)
    {
        InCombat = false;
    }

    public virtual void OnProjectileHit(Projectile p, Vector2 hitpoint)
    {
        p.OnTargetHit(this, hitpoint);

        // Instantiate(HitParticles, transform.position, Quaternion.identity);
    }
}

public class DamageMetaData
{
    public Projectile Projectile;
}