using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityTimer;
using System;
using EPOOutline;

public abstract class Monster : Unit, IMoving
{
    [field: SerializeField]
    public EnemyData EnemyData { get; set; }

    [SerializeField]
    private AudioClip deathSound;

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

    [HideInInspector] public float distanceTravelled;

    [HideInInspector] public List<Tower> targetedBy = new List<Tower>();

    public Timer StunTimer { get; set; }

    // Protected Fields
    protected List<Stat> stats;

    protected MonsterManager m;

    protected Transform myTransform;

    protected Vector3 endPosition;
    protected Vector3 startPosition;

    // Private Fields

    public Path CurrentPath { get; set; }
    public int CurrentWaypoint { get; set; }
    public bool HasPathAssigned => CurrentPath != null;

    private float progress = 0;
    private float previousX;
    private float previousY;
    private float deltaX = 0;
    private float deltaY = 0;
    private float pathLength;
    private float step = 0;

    private Timer _directionSwitchTimer;

    protected override void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();

        myTransform = transform;
        m = ValueStore.Instance.monsterManagerInstance;

        InitializeValues();
    }

    protected override void Start()
    {
        base.Start();

        AdjustDirection();
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
        CurrentPath = path;
        startPosition = CurrentPath.waypoints[CurrentWaypoint].transform.position;
        endPosition = CurrentPath.waypoints[CurrentWaypoint + 1].transform.position;
        pathLength = Vector3.Distance(startPosition, endPosition);
    }

    public virtual void FixedUpdate()
    {
        if (!IsDead && HasPathAssigned)
        {
            FollowPath();
        }
    }

    protected virtual void Update()
    {

    }

    public void FollowPath()
    {
        if (CurrentWaypoint < CurrentPath.waypoints.Count - 1)
        {
            float speed = MoveSpeed.Value * Time.deltaTime;
            step = speed / pathLength;
            distanceTravelled += speed;
            progress += step;
        }
        myTransform.root.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp(progress, progress, 1));

        deltaX = Mathf.Abs(endPosition.x - startPosition.x);
        deltaY = Mathf.Abs(endPosition.y - startPosition.y);

        if (_directionSwitchTimer.GetTimeRemaining() <= 0)
        {
            AdjustDirection();
            _directionSwitchTimer.Restart(0.4f);
        }

        previousX = myTransform.position.x;
        previousY = myTransform.position.y;

        if (myTransform.root.position == endPosition)
        {
            progress = 0;
            CurrentWaypoint++;
            AdjustDirection();
            startPosition = endPosition;
            endPosition = CurrentPath.waypoints[CurrentWaypoint + 1].transform.position;
            pathLength = Mathf.Abs(Vector3.Distance(startPosition, endPosition));
        }
    }

    public void AdjustDirection()
    {
        float wdeltaX = endPosition.x - startPosition.x;
        float wdeltaY = endPosition.y - startPosition.y;

        transform.rotation = Quaternion.AngleAxis(180, Vector3.up);

        if (endPosition.x >= startPosition.x && (deltaX >= deltaY * 0.8f))
        {
            side.SetActive(true);
            down.SetActive(false);
            up.SetActive(false);
        }
        else if (endPosition.x < startPosition.x && (deltaX > deltaY * 0.8f))
        {
            side.SetActive(true);
            down.SetActive(false);
            up.SetActive(false);
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        // else if (endPosition.y >= startPosition.y && (deltaY >= deltaX))
        // {
        //     up.SetActive(true);
        //     side.SetActive(false);
        //     down.SetActive(false);
        // }
        // else
        // {
        //     down.SetActive(true);
        //     side.SetActive(false);
        //     up.SetActive(false);
        // }
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
                t = targetedBy.FirstOrDefault(x => ValueStore.Instance.monsterManagerInstance.DoesKill(this, x.AD.Value, x.AP.Value));
            }
            else
            {
                t = targetedBy.FirstOrDefault(x => x.monstersInRange.Count == 1 && ValueStore.Instance.monsterManagerInstance.DoesKill(this, x.AD.Value, x.AP.Value));
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

    protected override void Die(DamageSource source, IAttacking killer)
    {
        if (IsDead)
        {
            base.Die(source, killer);
            return;
        }

        m.OnEnemyDied(this, source);

        anim.SetTrigger("death");
        anim.SetInteger("death_index", UnityEngine.Random.Range(0, 2));

        _audioSource.pitch = UnityEngine.Random.Range(0, 2f);
        _audioSource.PlayOneShot(deathSound);

        // Add death particle effects
        // GameObject deathParticle = (GameObject)Instantiate(m.deathParticlePrefab, myTransform.position, myTransform.rotation);
        // Destroy(deathParticle, 2f);

        base.Die(source, killer);
    }
}