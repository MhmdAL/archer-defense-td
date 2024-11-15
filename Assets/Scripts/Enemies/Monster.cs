using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityTimer;
using System;
using EPOOutline;
using UnityEngine.AddressableAssets;
using DG.Tweening;

public abstract class Monster : Unit, IMoving
{
    [field: SerializeField]
    public EnemyData EnemyData { get; set; }

    // private AudioComposition deathSound;

    public Stat MoveSpeed { get; set; }

    public List<Stat> Stats
    {
        get
        {
            return stats;
        }
        set
        {
            stats = value;
        }
    }

    public MovementTracker movementTracker;

    [Header("Objects for different directions")]
    public GameObject side;
    public GameObject up;
    public GameObject down;
    public GameObject stunImage;

    public Sprite icon;

    public List<GameObject> colorChangeBlockList = new List<GameObject>();

    public EnemyType enemyType;

    public int ID;

    public Animator anim;
    public AudioSource _audioSource;

    [HideInInspector] public bool isAboutToDie;

    public float DistanceTravelled => _followPathComponent.DistanceTravelled;

    [HideInInspector] public List<Tower> targetedBy = new List<Tower>();

    public Timer StunTimer { get; set; }

    // Protected Fields
    protected List<Stat> stats;

    protected MonsterManager m;

    protected Transform myTransform;

    // Private Fields

    public PathData CurrentPath => _followPathComponent?.CurrentPath;
    public int CurrentWaypoint { get; set; }
    public bool HasPathAssigned => CurrentPath != null;

    private Timer _directionSwitchTimer;

    private FollowPath _followPathComponent;

    protected override async void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();
        _followPathComponent = GetComponent<FollowPath>();

        _followPathComponent.TargetWaypointChanged += OnTargetChanged;
        movementTracker.MovementChanged += OnMovement;

        myTransform = transform;
        // m = ValueStore.Instance.monsterManagerInstance;

        InitializeValues();
    }

    public virtual void InitializeValues()
    {
        _directionSwitchTimer = Timer.Register(0, null, isDoneWhenElapsed: false);

        StunTimer = Timer.Register(0, null, isDoneWhenElapsed: false);

        //currentColor = new MyColor (GetComponentInChildren<SpriteRenderer> ().color, 0);

        Armor = new Stat(Type.Armor, 0);
        MoveSpeed = new Stat(Type.MOVEMENT_SPEED, EnemyData.Movespeed);
        DamageModifier = new Stat(Type.DAMAGE_TAKEN, 1);

        stats = new List<Stat>() { MaxHP, Armor, MoveSpeed, DamageModifier };

        MaxHP.BaseValue = EnemyData.MaxHealth;
    }

    public virtual void SetPath(Path path)
    {
        _followPathComponent.SetPath(path.PathData, 0, true);
    }

    public virtual void FixedUpdate()
    {

    }

    // private float timer;

    protected virtual void Update()
    {
        // timer+=Time.deltaTime;

        // if(timer > 5)
        // {
        //     // print(transform.root.position);
        //     // print(movementTracker.CurrentVelocity);
        //     timer = -1000;
        // }
    }

    public GameObject footprintPrefab;

    public float footprintDistanceCounter = 0;

    private void OnMovement(Vector3 delta, Vector3 currentVelocity)
    {
        footprintDistanceCounter += delta.magnitude;

        if (footprintDistanceCounter >= UnityEngine.Random.Range(2, 4))
        {
            footprintDistanceCounter = 0;

            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x);

            Quaternion rotation = Quaternion.Euler(0f, 0f, 90 + angle * Mathf.Rad2Deg); // Convert from radians to degrees

            var footprint = Instantiate(footprintPrefab, transform.position, rotation);
            footprint.GetComponent<SpriteRenderer>().DOFade(0, 15f);
        }

        anim.SetFloat("walk_speed", currentVelocity.magnitude);

        UpdateDirection(delta);
    }

    public override void OnProjectileHit(Projectile p, Vector2 hitpoint)
    {
        base.OnProjectileHit(p, hitpoint);

        MoveSpeed.Modify(-0.1f, BonusType.Percentage, "MAAA", 0.2f, 1);

        anim.SetTrigger("hit");
    }

    private void OnTargetChanged(Vector2 newDirection)
    {
        UpdateDirection(newDirection);
    }

    private void UpdateDirection(Vector3 diff)
    {
        if (diff.x == 0)
        {

        }
        else if (diff.x < 0)
        {
            // Moving left
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if (diff.x > 0)
        {
            // Moving right
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    public void RecheckTargeter()
    {
        if (targetedBy.Count > 1 && isAboutToDie)
        {
            Tower t;// t = targetedBy.FirstOrDefault (x => x.bulletsOut.Count > 0 && x.bulletsOut.FirstOrDefault (b => b.target == transform && b.isAboutToKill));
                    //if (t == null) {
                    //t = targetedBy.FirstOrDefault (x => x.monstersInRange.Count == 1);
            if (targetedBy.All(x => x.monstersInRange.Count > 1))
            {
                t = targetedBy.FirstOrDefault(x => MonsterManager.DoesKill(this, x.AD.Value, x.AP.Value));
            }
            else
            {
                t = targetedBy.FirstOrDefault(x => x.monstersInRange.Count == 1 && MonsterManager.DoesKill(this, x.AD.Value, x.AP.Value));
            }
            //}
            if (t != null)
            {
                List<Tower> l = targetedBy;
                l.Remove(t);
                foreach (Tower to in l.ToList())
                {
                    targetedBy.Remove(to);
                    //	Debug.Log (t);
                    to.RecheckTarget();
                }
            }
        }
    }

    protected override void Die(DamageSource source, IAttacking killer, DamageMetaData damageMeta)
    {
        if (IsDead)
        {
            base.Die(source, killer, damageMeta);
            return;
        }

        // m.OnEnemyDied(this, source);

        anim.SetTrigger("death");

        if (damageMeta?.Projectile != null)
        {
            var diff = damageMeta.Projectile.transform.position.x - transform.position.x;

            anim.SetInteger("death_index", diff < 0 ? 0 : 1); // index 0: right death animation, index 1: left death animation
        }
        else
        {
            anim.SetInteger("death_index", UnityEngine.Random.Range(0, 2));
        }

        _audioSource.PlayOneShot(SoundEffects.ENEMY_DEATH);

        // Add death particle effects
        // GameObject deathParticle = (GameObject)Instantiate(m.deathParticlePrefab, myTransform.position, myTransform.rotation);
        // Destroy(deathParticle, 2f);

        _followPathComponent.enabled = false;

        base.Die(source, killer, damageMeta);
    }

    public void OnMovementStarted()
    {

    }

    public void OnMovementEnded()
    {

    }

    private void OnDestroy()
    {
        if (_followPathComponent)
        {
            // _followPathComponent.OnMovementChanged -= OnMovementChanged;
        }
    }
}