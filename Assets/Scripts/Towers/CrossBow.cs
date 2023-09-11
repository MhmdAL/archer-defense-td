using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityTimer;

public class CrossBow : MonoBehaviour, IFocusable, IShooting, IAttacking
{
    public SpriteRenderer SpearSprite;
    public Timer AttackTimer;
    public float AttackCooldown = 1;
    public float AttackDamage = 10;
    public float ProjectileDuration = 1f;
    public Projectile BoltPrefab;

    public bool HasFocus { get; set; }

    [SerializeField]
    private List<TargetHitEffect> _onHitEffects;
    public List<TargetHitEffect> OnHitEffects => _onHitEffects;

    private Outlinable _outlinable;

    private void Awake()
    {
        _outlinable = GetComponentInChildren<Outlinable>();

        AttackTimer = this.AttachTimer(0, OnAttackTimerElapsed, isDoneWhenElapsed: false);
    }

    private void Update()
    {
        if (HasFocus && Input.GetKeyDown(KeyCode.F) && AttackTimer.GetTimeRemaining() <= 0)
        {
            Attack();
        }
    }

    private void OnAttackTimerElapsed(Timer t)
    {
        t.Pause();

        SpearSprite.enabled = true;
    }

    private void Attack()
    {
        SpearSprite.enabled = false;
        AttackTimer.Restart(AttackCooldown);
        AttackTimer.Resume();

        var targetPosition = Input.mousePosition.ToWorldPosition(Camera.main);
        targetPosition.z = 0;

        Projectile.Fire(new ProjectileSpawnData(this, BoltPrefab, transform.position, targetPosition)
        {
            Radius = 0,
            Duration = ProjectileDuration,
            Damage = AttackDamage,
            LingerTime = 2f
        });
    }

    public void Focus()
    {
        HasFocus = true;

        _outlinable.OutlineParameters.Enabled = true;
    }

    public void UnFocus()
    {
        HasFocus = false;

        _outlinable.OutlineParameters.Enabled = false;
    }

    public void OnTargetHit(Vector3 TargetPosition, List<IProjectileTarget> unitsHit, Projectile p, int shotNumber)
    {
        foreach (var ohe in OnHitEffects)
        {
            ohe.OnTargetHit(new AttackData
            {
                HitPosition = TargetPosition,
                HitRadius = 0,
                Owner = this,
                Projectile = p,
                Damage = p.Damage,
                ArmorPen = p.ArmorPen,
                Targets = unitsHit
            });
        }
    }
}