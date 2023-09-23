using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityTimer;
using EPOOutline;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using DG.Tweening;

public class Tower : MonoBehaviour, IAttacking, IFocusable, IShooting, IMoving
{
    public event Action<Tower> SkillPointsChanged;
    public event Action SkillUpgraded;
    public event Action AttackFinished;
    public event Action CombatEnded;

    [field: SerializeField, Header("Data")]
    public TowerSkillData TowerSkillsData { get; set; }
    [field: SerializeField]
    public TowerData TowerData { get; set; }

    [field: SerializeField, Header("Attack Configuration")]
    public TowerAttackStrategy TowerAttackStrategy { get; set; }


    [field: SerializeField]
    public List<TargetHitEffect> OnHitEffects { get; set; }

    public Dictionary<string, object> ExtraData { get; set; }

    [field: SerializeField]
    public int SecondaryTargetCount { get; set; } = 0;

    public List<IEnhancement> Enhancements { get; set; }

    public ArcherType ArcherSpecialty { get; set; }

    public int SpecializationLevel { get; set; }


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

    public float UpgradeCost
    {
        get
        {
            return baseUpgradeCost + (10 * level);
        }
        set
        {
            upgradeCost = value;
        }
    }

    protected List<Stat> stats;

    public GameObject upgradeAnimationPrefab;
    public GameObject archerShotParticle;

    [AssetsOnly]
    public GameObject bullet;
    public GameObject circle;
    public GameObject cooldownBar;

    public SpriteRenderer archerTowerRenderer;

    public BuffIndicatorPanel buffIndicatorPanel;

    public Transform upgradeAnimSpawnPoint;
    public Transform arrowSpawnPoint;


    public Stat AD { get; set; }
    public Stat AS { get; set; }
    public Stat AR { get; set; }
    public Stat AP { get; set; }
    public Stat MoveSpeed { get; set; }

    public Timer AttackCooldownTimer { get; set; }
    public Timer CombatTimer { get; set; }

    [field: SerializeField]
    public float CombatCooldown { get; set; }

    public TowerSkill ADSkill { get; set; }
    public TowerSkill ARSkill { get; set; }
    public TowerSkill ASSkill { get; set; }

    public float baseUpgradeCost;
    public float bulletSpeed;
    public float bulletLinger;
    public float bulletRadius;

    public Sprite icon;

    public string title;

    public int maxLevel;

    public TextAsset archerUpgrades;

    public TowerBase TowerBase { get; set; }

    public bool IsDisabled { get; set; }

    public float FullCooldown => 1 / AS.Value;

    [HideInInspector]
    public float silverSpent;

    [HideInInspector]
    public int cost;

    [HideInInspector]
    public int level;

    [HideInInspector]
    public List<Monster> monstersInRange;

    protected Animator anim;

    [SerializeField]
    protected Animator animator;

    private Outlinable _outlinable;


    public int consecutiveShots;
    public int shotNumber;

    public GameObject FocusIndicatorArrow;

    private List<Monster> monstersInScene;
    private List<Monster> targets;

    public List<Modifier> towerUpgrades = new List<Modifier>();

    private float upgradeCost;

    private Transform cooldownBarTransform;
    private Transform rangeCircleTransform;

    private Vector3 rangeCircleScale;
    private Vector3 cooldownBarScale;

    private GameObject cooldownBarParent;

    public bool isInCombat = false;

    private List<SpriteRenderer> attachedRenderers;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioSource walkingAudioSource;

    [SerializeField, Header("Audio")]
    private AudioClip ShootSFX;
    [SerializeField]
    private AudioClip DrawSFX;
    [SerializeField]
    private AudioClip HitSFX;
    [SerializeField]
    private AudioClip WalkingSFX;

    private int _skillPoints;
    public int SkillPoints
    {
        get
        {
            return _skillPoints;
        }
        set
        {
            _skillPoints = value;

            SkillPointsChanged?.Invoke(this);
        }
    }

    public int CurrentSkillLevel { get; set; }

    private int _currentXP;
    public int CurrentXP
    {
        get
        {
            return _currentXP;
        }
        set
        {
            _currentXP = value;

            if (TowerSkillsData.SkillValues.Count > CurrentSkillLevel)
            {
                var nextLevelXp = TowerSkillsData.SkillValues[CurrentSkillLevel];

                if (CurrentXP >= nextLevelXp.ExpRequired)
                {
                    SkillPoints++;

                    CurrentSkillLevel++;
                }
            }
        }
    }

    [SerializeField]
    private LineRenderer lineRenderer;

    public bool HasFocus { get; set; }

    private void Awake()
    {
        Enhancements = new List<IEnhancement>();

        ExtraData = new Dictionary<string, object>();

        stats = new List<Stat>() { AD, AS, AR, AP, MoveSpeed };

        // Initalize cooldown timers
        InitializeValues();


        // Add modifiers that apply from the start
        AddStartingModifiers();

        monstersInRange = new List<Monster>();
        targets = new List<Monster> { null, null };

        // Set values from file, used when upgrading tower
        SetUpgradeValues();

        anim = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        _outlinable = GetComponentInChildren<Outlinable>();
    }

    public virtual void InitializeValues()
    {
        silverSpent = cost;

        AttackCooldownTimer = this.AttachTimer(0, OnAttackTimerElapsed, isDoneWhenElapsed: false);
        CombatTimer = this.AttachTimer(2f, OnCombatTimerElapsed, isDoneWhenElapsed: false);

        cooldownBarTransform = cooldownBar.transform;
        rangeCircleTransform = circle.transform;

        rangeCircleTransform.position = transform.position;

        cooldownBarScale = cooldownBarTransform.localScale;
        rangeCircleScale = rangeCircleTransform.localScale;

        cooldownBarParent = cooldownBarTransform.parent.gameObject;

        AD = new Stat(Type.ATTACK_DAMAGE, TowerData.BaseAttackDamage * (1 + SaveData.GetUpgrade(UpgradeType.AD)?.CurrentValue ?? 0));
        AS = new Stat(Type.ATTACK_SPEED, TowerData.BaseAttackSpeed * (1 + SaveData.GetUpgrade(UpgradeType.AS)?.CurrentValue ?? 0));
        AR = new Stat(Type.ATTACK_RANGE, TowerData.BaseAttackRange * (1 + SaveData.GetUpgrade(UpgradeType.AR)?.CurrentValue ?? 0));
        AP = new Stat(Type.ARMOR_PENETRATION, SaveData.GetUpgrade(UpgradeType.AP)?.CurrentValue ?? 0);
        MoveSpeed = new Stat(Type.MOVEMENT_SPEED, TowerData.BaseMoveSpeed);

        ADSkill = new TowerSkill { SkillType = TowerSkillType.AttackDamage };
        ARSkill = new TowerSkill { SkillType = TowerSkillType.AttackRange };
        ASSkill = new TowerSkill { SkillType = TowerSkillType.AttackSpeed };

        UpdateTowerVisuals();
    }

    public virtual void AddStartingModifiers() { }

    private void Update()
    {
        UpdateTowerVisuals();

        if (monstersInRange.Any() && AttackCooldownTimer.GetTimeRemaining() > FullCooldown - 0.1f)
        {
            PlayBowDrawSFX();
        }

        var res = Physics2D.OverlapCircleAll(transform.position, AR.Value);
        foreach (var obj in res)
        {
            var enemy = obj.GetComponent<Monster>();
            if (enemy)
            {
                OnEnemyInRange();
                break;
            }
        }
    }

    [SerializeField]
    private ParticleSystem dustPs;

    public void OnMovementStarted()
    {
        if (!walkingAudioSource.isPlaying)
        {
            walkingAudioSource.clip = WalkingSFX;
            walkingAudioSource.Play();
        }
        else
        {
            walkingAudioSource.DOFade(.25f, .2f);
        }

        dustPs.Play();
    }

    public void OnMovementEnded()
    {
        walkingAudioSource.DOFade(0, .2f);

        dustPs.Stop();
    }

    private void OnEnemyInRange()
    {
        CombatTimer.Restart(CombatCooldown);
        AttackCooldownTimer.Resume();

        // print("combat restarted");

        // animator.SetBool("idle", false);
        animator.SetBool("isAttacking", true);
    }

    private void OnCombatTimerElapsed(Timer t)
    {
        isInCombat = false;
        consecutiveShots = 0;

        // print("combat elapsed");

        // animator.SetBool("idle", true);
        animator.SetBool("isAttacking", false);

        AttackCooldownTimer.Restart(FullCooldown);
        AttackCooldownTimer.Pause();

        CombatEnded?.Invoke();
    }

    private void OnAttackTimerElapsed(Timer t)
    {
        monstersInScene = ValueStore.Instance.monsterManagerInstance.MonstersInScene.ToList();
        TargetDetection.CalculateTargets(this, monstersInScene, monstersInRange, targets, AR.Value, AD.Value, AP.Value);

        Attack();
    }

    private void Attack()
    {
        if (!IsDisabled)
        {
            var secondaryTargets = monstersInRange.ToList();

            if (targets.Count > 0)
                secondaryTargets.Remove(targets.FirstOrDefault());

            secondaryTargets = secondaryTargets.Take(SecondaryTargetCount).ToList();

            TowerAttackStrategy.Attack(new TowerAttackData
            {
                Owner = this,
                PrimaryTarget = targets.FirstOrDefault(),
                SecondaryTargets = secondaryTargets,
                MonstersInRange = monstersInRange,
                OnAfterAttack = OnAfterAttack
            });
        }
        else
        {
            AttackCooldownTimer.Restart(FullCooldown);
        }
    }

    public void PlayShotSFX() => PlayClip(ShootSFX);
    public void PlayBowDrawSFX() => PlayClip(DrawSFX);
    public void PlayHitSound() => PlayClip(HitSFX);

    public void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void OnBeforeAttack()
    {

    }

    private void OnAfterAttack()
    {
        CurrentXP++;

        AttackFinished?.Invoke();
    }

    public virtual void OnTargetHit(Monster target, Projectile p, List<Monster> aoeTargets, int shotNumber)
    {
        foreach (var ohe in OnHitEffects)
        {
            ohe.OnTargetHit(new AttackData
            {
                Owner = this,
                Projectile = p,
                Targets = new List<IProjectileTarget> { target }
            });
        }
    }

    public void OnTargetHit(Vector3 targetPosition, List<IProjectileTarget> unitsHit, Projectile p, int shotNumber)
    {
        PlayHitSound();

        foreach (var ohe in OnHitEffects)
        {
            ohe.OnTargetHit(new AttackData
            {
                Owner = this,
                Projectile = p,
                Damage = p.Damage,
                ArmorPen = p.ArmorPen,
                HitPosition = targetPosition,
                HitRadius = bulletRadius,
                Targets = unitsHit
            });
        }
    }

    #region Target Calculation

    private void CalculateTargets()
    {
        // Detect monsters in range
        monstersInScene = ValueStore.Instance.monsterManagerInstance.MonstersInScene.ToList();
        monstersInRange.Clear();
        foreach (Monster x in monstersInScene)
        {
            if (IsInRange(x) && !monstersInRange.Contains(x))
            {
                monstersInRange.Add(x);
            }
            else if (!IsInRange(x) && monstersInRange.Contains(x))
            {
                monstersInRange.Remove(x);
            }
        }
        // Remove monsters who leave range from list of targets
        foreach (Monster t in targets.ToList())
        {
            if (!IsInRange(t) || t.IsDead)
            {
                if (targets[0] != null && targets[0].targetedBy.Count > 0)
                {
                    if (targets[0].targetedBy.FirstOrDefault(x => x == this))
                        targets[0].targetedBy.Remove(this);
                }
                targets[targets.IndexOf(t)] = null;
            }
        }
        // Set targets
        SetTargets();

        if (targets[0] != null)
        {
            if (ValueStore.Instance.monsterManagerInstance.DoesKill(targets[0], AD.Value, AP.Value))
            {
                targets[0].isAboutToDie = true;
            }
        }
    }

    public void SetTargets()
    {
        if (monstersInRange.Count > 0)
        { // If there is atleast 1 enemy in range
            monstersInRange.Sort((x, y) => y.distanceTravelled.CompareTo(x.distanceTravelled)); // Sort enemies in range by how far they travelled

            if (targets[0] == null && monstersInRange[0] != targets[1])
            { // If primary and secondary targets are null

                if (monstersInRange.Count > 1)
                { // If there is more than 1 enemy in range
                    Monster someTarget = monstersInRange.FirstOrDefault(x => x.isAboutToDie == false); // Try to target enemies that arent about to die
                    if (someTarget != null)
                    {
                        targets[0] = someTarget;
                    }
                    else
                    { // If non are found then default to the farthest enemy
                        targets[0] = monstersInRange[0];
                    }
                }
                else
                {
                    targets[0] = monstersInRange[0];
                }

            }
            else if (targets[0] == null && monstersInRange[0] == targets[1])
            { // If primary target is null and secondary target is not null
                targets[0] = monstersInRange[0]; // Set as primary instead of secondary
                targets[1] = null;
            }
            if (!targets[0].targetedBy.Contains(this))
            {
                targets[0].targetedBy.Add(this);
            }
            targets[0].RecheckTargeter(); // Make sure enemy isnt about to die
        }
    }

    public void RecheckTarget()
    {
        targets[0] = null;
        TargetDetection.SetTargets(this, monstersInRange, targets);
    }

    public bool IsInRange(Monster m)
    {
        if (m)
        {
            Vector3 targetDistance = m.transform.root.position - transform.position;
            if (targetDistance.magnitude < AR.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }

    #endregion

    public void ApplyEnhancement(IEnhancement enhancement)
    {
        Enhancements.Add(enhancement);

        enhancement.Apply(this);
    }

    public void UpgradeSkill(TowerSkillType skill)
    {
        if (CurrentSkillLevel - 1 < 0)
        {
            return;
        }

        SkillPoints--;

        var currentSkillValues = TowerSkillsData.SkillValues[CurrentSkillLevel - 1];

        switch ((TowerSkillType)skill)
        {
            case TowerSkillType.AttackDamage:
                AD.Modify(currentSkillValues.ADValue,
                 currentSkillValues.IsADPercentage ? BonusType.Percentage : BonusType.Flat,
                  BuffNames.TOWER_SKILL_AD + CurrentSkillLevel);

                ADSkill.CurrentLevel++;

                buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_DAMAGE);
                break;
            case TowerSkillType.AttackSpeed:
                AS.Modify(currentSkillValues.ASValue,
                 currentSkillValues.IsASPercentage ? BonusType.Percentage : BonusType.Flat,
                  BuffNames.TOWER_SKILL_AS + CurrentSkillLevel);

                ASSkill.CurrentLevel++;

                buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_SPEED);
                break;
            case TowerSkillType.AttackRange:
                AR.Modify(currentSkillValues.ARValue,
                 currentSkillValues.IsARPercentage ? BonusType.Percentage : BonusType.Flat,
                  BuffNames.TOWER_SKILL_AR + CurrentSkillLevel);

                ARSkill.CurrentLevel++;

                buffIndicatorPanel.AddIndicator(BuffIndicatorType.ATTACK_RANGE);
                break;
        }

        SkillUpgraded?.Invoke();
    }

    public virtual void Upgrade()
    {
        Instantiate(upgradeAnimationPrefab, upgradeAnimSpawnPoint.position, Quaternion.identity);
        if (level == 0)
        {
            for (int i = 0; i < towerUpgrades.Count; i++)
            {
                if (towerUpgrades[i].intendedLevel == level + 1)
                {
                    // AddModifier(towerUpgrades[i], StackOperation.Additive, 1);
                }
            }
        }
        else
        {
            for (int i = 0; i < towerUpgrades.Count; i++)
            {
                if (towerUpgrades[i].intendedLevel == level + 1)
                {
                    // GetModifier(towerUpgrades[i].name).value += towerUpgrades[i].value;
                }
            }
        }
        silverSpent += UpgradeCost;
    }

    public virtual void SetUpgradeValues()
    {
        if (ArcherSpecialty == ArcherType.ClassicArcher)
            return;

        string[] splitFile = new string[] { "\r\n", "\r", "\n" };
        string[] lines = archerUpgrades.text.Split(splitFile, StringSplitOptions.None);
        string[] upgrades = lines[(int)ArcherSpecialty - 1].Split('/');

        for (int x = 0; x < maxLevel; x++)
        {
            string[] upgradeTypes = upgrades[x].Split(',');

            for (int z = 0; z < upgradeTypes.Length; z++)
            {
                string[] upgradeValues = upgradeTypes[z].Split(';');
                towerUpgrades.Add(new Modifier(float.Parse(upgradeValues[3]), (Name)int.Parse(upgradeValues[1]),
                    (Type)int.Parse(upgradeValues[0]), (BonusType)int.Parse(upgradeValues[2]), x + 1));
            }
        }
    }

    public void StartPoison(Monster target, Projectile p)
    {
        ShopUpgrade poison = SaveData.GetUpgrade(UpgradeType.Poison_Arrows);
        StartCoroutine(DOT(poison.CurrentValue * p.Damage, target));
    }

    public IEnumerator ColorFade()
    {
        for (int i = 0; i < 10; i++)
        {
            archerTowerRenderer.color = Color.Lerp(archerTowerRenderer.color, Color.white, 0.1f * (i + 1));
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator DOT(float dmg, Monster target)
    {
        int tickCount = 10;

        for (int i = 0; i < tickCount; i++)
        {
            if (target != null)
            {
                target.Damage(dmg / 10, AP.Value, DamageSource.Normal, this);
            }
            else
            {
                yield return 0;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public float CurrentValue(UpgradeType upgrade)
    {
        return SaveData.GetUpgrade(upgrade).level * SaveData.GetUpgrade(upgrade).valuePerLevel;
    }

    public static bool CanUpgrade(Tower t)
    {
        if (t.ArcherSpecialty == ArcherType.ClassicArcher)
            return true;

        if (t.ArcherSpecialty == ArcherType.RapidArcher && SaveData.GetUpgrade(UpgradeType.Rapid_1).level >= t.level)
        {
            return true;
        }
        else if (t.ArcherSpecialty == ArcherType.LongArcher && SaveData.GetUpgrade(UpgradeType.Long_1).level >= t.level)
        {
            return true;
        }
        else if (t.ArcherSpecialty == ArcherType.UtilityArcher && SaveData.GetUpgrade(UpgradeType.Utility_1).level >= t.level)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateTowerVisuals()
    {
        var target = targets?.FirstOrDefault();

        if (target != null)
        {
            var dir = target.transform.position - transform.position;

            if (dir.x > 0)
            {
                animator.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            }
            else
            {
                animator.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            }
        }

        animator.SetFloat("attackProgress", 1 - Mathf.Clamp01(AttackCooldownTimer.GetTimeRemaining() / FullCooldown));

        if (!IsDisabled && monstersInRange.Count > 0)
        { // If tower is active and monsters nearby, update cooldown bar
            cooldownBarScale = new Vector3(1, Mathf.Clamp(AttackCooldownTimer.GetTimeRemaining() / FullCooldown, 0, 1), 1);
        }
        else
        {
            cooldownBarScale = new Vector3(1, 0, 1);
        }
        cooldownBarScale = new Vector3(1, Mathf.Clamp(AttackCooldownTimer.GetTimeRemaining() / FullCooldown, 0, 1), 1);
        cooldownBarTransform.localScale = cooldownBarScale;

        // Set the scale of the range circle to the value of attack range
        rangeCircleScale.y = AR.Value / 2;
        rangeCircleScale.x = AR.Value / 2;

        rangeCircleTransform.localScale = rangeCircleScale;

        DrawCircle();
    }

    private void DrawCircle()
    {
        lineRenderer.positionCount = 501;

        for (int currentStep = 0; currentStep <= 500; currentStep++)
        {
            float circProgress = (float)currentStep / 500;

            float currentRadian = circProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * AR.Value;
            float y = yScaled * AR.Value;

            var curPos = new Vector3(x, y, 0);

            lineRenderer.SetPosition(currentStep, curPos + transform.position);
        }
    }

    public void Focus()
    {
        HasFocus = true;

        _outlinable.OutlineParameters.Enabled = true;

        circle.SetActive(true);
        // cooldownBarParent.SetActive(true);

        FocusIndicatorArrow.SetActive(true);
    }

    public void UnFocus()
    {
        HasFocus = false;

        _outlinable.OutlineParameters.Enabled = false;

        circle.SetActive(false);
        // cooldownBarParent.SetActive(false);

        FocusIndicatorArrow.SetActive(false);
    }
}