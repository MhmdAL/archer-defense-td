using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class CrossBow : MonoBehaviour, IFocusable, IShooter, IAttacker
{
    public GameObject FocusIndicator;
    public SpriteRenderer SpearSprite;
    public Timer AttackTimer;
    public float AttackCooldown = 1;
    public float AttackDamage = 10;
    public Projectile BoltPrefab;

    public bool HasFocus { get; set; }

    [SerializeField]
    private List<TargetHitEffect> _onHitEffects;
    public List<TargetHitEffect> OnHitEffects => _onHitEffects;

    private void Awake()
    {
        AttackTimer = this.AttachTimer(0, OnAttackTimerElapsed, isDoneWhenElapsed: false);
    }

    private void Update()
    {
        if (HasFocus && Input.GetKeyDown(KeyCode.Space) && AttackTimer.GetTimeRemaining() <= 0)
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
            Duration = .3f,
            Damage = AttackDamage,
            LingerTime = 2f
        });
    }

    public void Focus()
    {
        HasFocus = true;

        FocusIndicator.SetActive(true);
    }

    public void UnFocus()
    {
        HasFocus = false;

        FocusIndicator.SetActive(false);
    }

    public void OnTargetHit(Vector3 TargetPosition, List<Unit> unitsHit, Projectile p, int shotNumber)
    {
        foreach (var ohe in OnHitEffects)
        {
            ohe.OnTargetHit(new TargetHitData
            {
                HitPosition = TargetPosition,
                HitRadius = 0,
                Owner = this,
                Projectile = p,
                Targets = unitsHit
            });
        }
    }
}