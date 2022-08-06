using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityTimer;
using System;

public abstract class Monster : Unit, IModifiable
{
    public delegate void ModifierEndedEventHandler(Modifier m);
    public event ModifierEndedEventHandler ModifierEnded;

    [field: SerializeField]
    public EnemyData EnemyData { get; set; }


    public Stat Movespeed { get; set; }

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

    public List<Modifier> Modifiers
    {
        get
        {
            return modifiers;
        }
        set
        {
            modifiers = value;
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

    [HideInInspector] public bool isAboutToDie;

    [HideInInspector] public float distanceTravelled;

    [HideInInspector] public List<Tower> targetedBy = new List<Tower>();

    public Timer StunTimer { get; set; }

    // Protected Fields
    protected List<Stat> stats;

    protected List<Modifier> modifiers = new List<Modifier>();

    protected MonsterManager m;

    protected Transform myTransform;

    protected Vector3 endPosition;
    protected Vector3 startPosition;

    // Private Fields

    public Path CurrentPath { get; set; }
    public int CurrentWaypoint { get; set; }

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

        myTransform = transform;
        m = ValueStore.Instance.monsterManagerInstance;

        ModifierEnded += OnModifierEnded;

        InitializeValues();
    }

    protected override void Start()
    {
        base.Start();

        AdjustStats();

        AdjustDirection();
    }

    public virtual void InitializeValues()
    {
        _directionSwitchTimer = Timer.Register(0, null, isDoneWhenElapsed: false);

        StunTimer = Timer.Register(0, null, isDoneWhenElapsed: false);

        //currentColor = new MyColor (GetComponentInChildren<SpriteRenderer> ().color, 0);

        Armor = new Stat(Type.Armor);
        Movespeed = new Stat(Type.MOVEMENT_SPEED);
        DamageModifier = new Stat(Type.DAMAGE_TAKEN);

        stats = new List<Stat>() { MaxHP, Armor, Movespeed, DamageModifier };
        DamageModifier.BaseValue = 1;

        MaxHP.BaseValue = EnemyData.MaxHealth;
        Movespeed.BaseValue = EnemyData.Movespeed;
    }

    public virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected virtual void Update()
    {
        AdjustStats();
    }

    /*
	public void AdjustStats2(){
		List<Modifier> banList = new List<Modifier> ();
		float curSpdMod = 1;
		float curDmgTakenMod = 1;
		float curArmorMod = 1;
		if (modifiers.Count > 0) {
			foreach (Modifier x in modifiers) {
				if (x.active == false) {
					banList.Add (x);
				} else {
					if (x.type == Type.MOVEMENT_SPEED) {
						curSpdMod += x.value;
						if (GetModifier (Name.Utility_Stun) != null)
							curSpdMod = 0;
					} else if (x.type == Type.DAMAGE_TAKEN) {
						curDmgTakenMod += x.value;
					} else if (x.type == Type.Armor) {
						curArmorMod += x.value;
					}
				}
			}
		}
		foreach (Modifier m in banList) {
			modifiers.Remove (m);
			ModifierEnded (m);
		}
		//speed = Mathf.Clamp(baseMoveSpeed * curSpdMod, 0, 10);

		damageModifier = 1 * curDmgTakenMod;
	}*/

    public void FixedMovespeed(float value, float duration)
    {
        Movespeed.Value = value;
        Movespeed.locked = true;
        MyTimer t = ValueStore.Instance.timerManagerInstance.StartTimer(duration);
        t.TimerElapsed += FixedMoveSpeedEnded;
    }

    public void FixedMoveSpeedEnded()
    {
        Movespeed.locked = false;
    }

    protected override void OnDeath(DamageSource source, IAttacker killer)
    {
        if (IsDead)
        {
            base.OnDeath(source, killer);
            return;
        }

        m.OnEnemyDied(this, source);

        // Add death particle effects
        GameObject deathParticle = (GameObject)Instantiate(m.deathParticlePrefab, myTransform.position, myTransform.rotation);
        Destroy(deathParticle, 2f);

        base.OnDeath(source, killer);
    }

    public virtual void SetPath(int entrance, int exit)
    {
        List<Path> relevantPaths;
        if (entrance != 0 && exit != 0)
        {
            relevantPaths = m.paths.Where(x => x.entrance == entrance && x.exit == exit).ToList();
        }
        else if (entrance == 0 && exit != 0)
        {
            relevantPaths = m.paths.Where(x => x.exit == exit).ToList();
        }
        else if (entrance != 0 && exit == 0)
        {
            relevantPaths = m.paths.Where(x => x.entrance == entrance).ToList();
        }
        else
        {
            relevantPaths = m.paths.ToList();
        }
        int random = UnityEngine.Random.Range(1, relevantPaths.Count + 1);
        CurrentPath = relevantPaths[random - 1];
        startPosition = CurrentPath.waypoints[CurrentWaypoint].transform.position;
        endPosition = CurrentPath.waypoints[CurrentWaypoint + 1].transform.position;
        pathLength = Vector3.Distance(startPosition, endPosition);
    }

    public void HandleMovement()
    {
        if (CurrentWaypoint < CurrentPath.waypoints.Count - 1)
        {
            float speed = Movespeed.Value * Time.deltaTime;
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
            startPosition = CurrentPath.waypoints[CurrentWaypoint].transform.position;
            endPosition = CurrentPath.waypoints[CurrentWaypoint + 1].transform.position;
            pathLength = Mathf.Abs(Vector3.Distance(startPosition, endPosition));
        }
    }

    public void AdjustDirection()
    {
        float wdeltaX = endPosition.x - startPosition.x;
        float wdeltaY = endPosition.y - startPosition.y;

        side.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);

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
            side.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if (endPosition.y >= startPosition.y && (deltaY >= deltaX))
        {
            up.SetActive(true);
            side.SetActive(false);
            down.SetActive(false);
        }
        else
        {
            down.SetActive(true);
            side.SetActive(false);
            up.SetActive(false);
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

    public void OnModifierEnded(Modifier m)
    {

    }

    public void AddModifier(Modifier m, StackOperation s, int stackLimit)
    {
        if (GetModifier(m.name) != null)
        { // if Modifier already exists
            Modifier x = GetModifier(m.name);
            if (s == StackOperation.Additive)
            {
                if (stackLimit != 0)
                {
                    // if still didnt reach stack limit
                    if (x.currentStack < stackLimit)
                    {
                        // inc value and reset duration
                        x.value += m.value;
                        if (m.lifetime == ModifierLife.Temporary)
                        {
                            x.cdTimer.Duration = m.cdTimer.Duration;
                        }
                        x.currentStack++;
                    }
                    else
                    {
                        if (m.lifetime == ModifierLife.Temporary)
                        {
                            x.cdTimer.Duration = m.cdTimer.Duration;
                        }
                    }
                }
                else if (stackLimit == 0)
                { // if stacklimit is 0 then it stacks infinitly
                    x.value += m.value;
                    if (m.lifetime == ModifierLife.Temporary)
                    {
                        x.cdTimer.Duration = m.cdTimer.Duration;
                    }
                }
            }
            else if (s == StackOperation.HighestValue)
            {
                float mA = m.value < 0 ? -m.value : m.value;
                float xA = x.value < 0 ? -x.value : x.value;

                if (mA > xA)
                { // if passed modifier is higher value than current
                    x.value = m.value;
                    if (m.lifetime == ModifierLife.Temporary)
                    {
                        x.cdTimer.Duration = m.cdTimer.Duration;
                    }
                }
                else if (mA == xA)
                {
                    if (m.lifetime == ModifierLife.Temporary)
                    {
                        x.cdTimer.Duration = m.cdTimer.Duration;
                    }
                }
            }
        }
        else
        { // add modifier if it doesnt exist
            modifiers.Add(m);
            GetModifier(m.name).currentStack++;
        }
    }

    public void RemoveModifier(Name name)
    {
        foreach (Modifier x in modifiers)
        {
            if (x.name == name)
                modifiers.Remove(x);
        }
    }

    public Modifier GetModifier(Name name)
    {
        return modifiers.FirstOrDefault(x => x.name == name);
    }

    public virtual void AdjustStats()
    {
        // TODO: Fix this mess of a method..
        List<Modifier> banList = new List<Modifier>();
        foreach (var item in stats)
        {
            item.multiplier = 0;
            item.overallMultiplier = 1;
            item.flatBonus = 0;
        }
        if (modifiers.Count > 0)
        {
            foreach (Modifier m in modifiers)
            {
                if (m.active == false)
                    banList.Add(m);
                if (m.active == true)
                {
                    Stat s = stats.FirstOrDefault(x => x.type == m.type);
                    for (int i = 0; i < m.currentStack; i++)
                    {
                        m.Apply(this);
                    }
                    if (s != null)
                    {
                        if (m.bonusOperation == BonusOperation.Percentage)
                        {
                            s.multiplier += m.value;
                        }
                        else if (m.bonusOperation == BonusOperation.Flat)
                        {
                            s.flatBonus += m.value;
                        }
                        else if (m.bonusOperation == BonusOperation.OverallMultiplier)
                        { // <--- temporary If
                            s.overallMultiplier = m.value;
                        }
                    }
                }
            }
        }
        foreach (Modifier m in banList)
        {
            m.DeApply(this);
            modifiers.Remove(m);
            ModifierEnded(m);
        }

        foreach (var item in stats)
        {
            //item.BonusValue = (item.baseValue * item.multiplier) + item.flatBonus;
            //item.Value = item.baseValue + item.BonusValue;
            if (item.type == Type.MOVEMENT_SPEED)
            {
                item.Value = Mathf.Clamp(item.Value, 0, 10);
            }
        }

    }

    public IEnumerator ColorFade(List<SpriteRenderer> s)
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var item in s)
            {
                item.color = Color.Lerp(item.color, Color.white, 0.1f * (i + 1));
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnDestroy()
    {
        ModifierEnded -= OnModifierEnded;
    }
}

public enum DamageSource
{
    Normal,
    Exit
}

public class MyColor
{

    public Color color;
    public int importance;

    public MyColor(Color c, int importance)
    {
        this.color = color;
        this.importance = importance;
    }
}



public enum EnemyType
{
    Normal,
    Boss
}