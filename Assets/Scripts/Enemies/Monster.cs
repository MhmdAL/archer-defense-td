using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public enum EnemyType
{
	Normal,
	Boss
}
[System.Serializable]
public class ObjectComponent{
	public SpriteRenderer s;
	public int sortingOrder;

	public ObjectComponent(SpriteRenderer s, int sortingOrder){
		this.s = s;
		this.sortingOrder = sortingOrder;
	}
}

public class Monster : MonoBehaviour, IModifiable{

	// Public Fields
	public delegate void ModifierEndedEventHandler(Modifier m);
	public event ModifierEndedEventHandler ModifierEnded;

	public delegate void MonsterDamagedEventHandler(Monster m, float finalDamage, IAttacker source);
	public static event MonsterDamagedEventHandler MonsterDamaged;

	public List<Stat> Stats {
		get {
			return stats;
		}
		set {
			stats = value;
		}
	}

	public List<Modifier> Modifiers {
		get {
			return modifiers;
		}
		set {
			modifiers = value;
		}
	}

	[Header("Objects for different directions")]
	public GameObject side;
	public GameObject up;
	public GameObject down;
	public GameObject HPBar;
	public GameObject stunImage;

	public Sprite icon;

	public List<GameObject> colorChangeBlockList = new List<GameObject>();

	public EnemyType enemyType;

	public Canvas HPBarCanvas;

	public int ID;

	public Animator anim;

	[HideInInspector]	public Stat MaxHP = new Stat (Type.Health);
	[HideInInspector]	public Stat Armor = new Stat (Type.Armor);
	[HideInInspector]	public Stat MS = new Stat (Type.MOVEMENT_SPEED);
	[HideInInspector]	public Stat DamageModifier = new Stat(Type.DAMAGE_TAKEN);

	[HideInInspector]	public bool ded = false;
	[HideInInspector]	public bool inCombat = false;
	[HideInInspector]	public bool isAboutToDie;

	[System.NonSerialized]	public string name;
	[System.NonSerialized]	public string description;

	[HideInInspector]	public float silverValue;
	[HideInInspector] 	public float currentHealth;
	[HideInInspector] 	public float distanceTravelled;
	[HideInInspector]	public float combatTimer = 2;

	[HideInInspector]	public int livesValue;

	[HideInInspector]	public CooldownTimer stunnable = new CooldownTimer(0);
			
	[HideInInspector]	public List<Tower> targetedBy = new List<Tower> ();

	// Protected Fields
	protected List<Stat> stats;

	protected List<Modifier> modifiers = new List<Modifier> ();

	protected MonsterManager m;

	protected Transform myTransform;

	protected Vector3 endPosition;
	protected Vector3 startPosition;

	// Private Fields
	private List<ObjectComponent> components = new List<ObjectComponent>();

	private Path currentPath;

	private int currentWaypoint = 0;

	private float progress = 0;
	private float previousX; 
	private float previousY;
	private float deltaX = 0;
	private float deltaY = 0;
	private float pathLength;
	private float step = 0;

	private CooldownTimer CombatTimer;
	private CooldownTimer directionSwitchTimer;
	private CooldownTimer sortingOrderTimer;

    public virtual void Awake() {
		myTransform = transform;
		m = ValueStore.sharedInstance.monsterManagerInstance;

		ModifierEnded += OnModifierEnded;

		InitializeValues ();
    }

	void Start(){
		AdjustStats ();

		FixDirection ();

		currentHealth = MaxHP.Value;
	}

	public virtual void InitializeValues(){
		foreach (var item in myTransform.GetComponentsInChildren<SpriteRenderer>(true).ToList()) {
			components.Add (new ObjectComponent(item, item.sortingOrder));
		}

		CombatTimer = new CooldownTimer (combatTimer);
		directionSwitchTimer = new CooldownTimer (0);
		sortingOrderTimer = new CooldownTimer (0);

		//currentColor = new MyColor (GetComponentInChildren<SpriteRenderer> ().color, 0);
			
		stats = new List<Stat> (){ MaxHP, Armor, MS, DamageModifier };
		DamageModifier.BaseValue = 1;

		MaxHP.BaseValue = MaxHP.BaseValue * m.CurrentMultiplier * 0.95f;
		Armor.BaseValue = Armor.BaseValue * m.CurrentMultiplier * 0.95f;
	}

    public virtual void FixedUpdate()
    {
		HandleMovement ();

		if (sortingOrderTimer.GetCooldownRemaining () <= 0) {
			AdjustSortingOrder ();
			sortingOrderTimer.ResetTimer (0.1f);
		}
    }

	void Update(){
		HPBar.transform.localScale = new Vector3(currentHealth/MaxHP.Value, HPBar.transform.localScale.y, HPBar.transform.localScale.z);

		AdjustStats ();
	
		if (CombatTimer.GetCooldownRemaining() <= 0) {
			inCombat = false;
		}
	}
		
	public virtual void AdjustStats(){
		// TODO: Fix this mess of a method..
		List<Modifier> banList = new List<Modifier>();
		foreach (var item in stats) {
			item.multiplier = 0;
			item.overallMultiplier = 1;
			item.flatBonus = 0;
		}
		if (modifiers.Count > 0) {
			foreach (Modifier m in modifiers) {
				if (m.active == false)
					banList.Add (m);
				if (m.active == true) {
					Stat s = stats.FirstOrDefault (x => x.type == m.type);
					for (int i = 0; i < m.currentStack; i++) {
						m.Apply (this);
					}
					if (s != null) {
						if (m.bonusOperation == BonusOperation.Percentage) {
							s.multiplier += m.value;
						} else if (m.bonusOperation == BonusOperation.Flat) {
							s.flatBonus += m.value;
						} else if (m.bonusOperation == BonusOperation.OverallMultiplier) { // <--- temporary If
							s.overallMultiplier = m.value;
						}
					}
				}
			}
		}
		foreach (Modifier m in banList) {
			m.DeApply (this);
			modifiers.Remove (m);
			ModifierEnded (m);
		}

		foreach (var item in stats) {
			//item.BonusValue = (item.baseValue * item.multiplier) + item.flatBonus;
			//item.Value = item.baseValue + item.BonusValue;
			if (item.type == Type.MOVEMENT_SPEED) {
				item.Value = Mathf.Clamp (item.Value, 0, 10);
			}
		}

	}

	public void AdjustSortingOrder(){
		for (int i = 0; i < components.Count; i++) {
			if (components [i].s != null) {
				components [i].s.sortingOrder = Mathf.RoundToInt (myTransform.root.position.y * -150) + components [i].sortingOrder;
				HPBarCanvas.sortingOrder = Mathf.RoundToInt ((myTransform.root.position.y) * -150);
			}
		}
	}

	public void ChangeColor(MyColor c){
		/*if(c.importance > currentColor.importance){
			previousColor = currentColor;
			currentColor = c;
		}
		foreach (var item in m.GetComponentsInChildren<SpriteRenderer>()) {
			item.color = currentColor.color;
		}*/
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

	public virtual void Damage(float damage, float armorpen, DamageSource source, IAttacker killer)
	{
		inCombat = true;
		CombatTimer.ResetTimer (combatTimer);
		float modifiedDamage = damage * DamageModifier.Value;
		float finalDamage = 0;
		if (this != null) {
			if (currentHealth > 0) {
			//	if (Armor.Value - armorpen > 0) {
				armorpen = Mathf.Clamp(armorpen, 0, 1);
					finalDamage = (float)(modifiedDamage * (50 / (50 + (Armor.Value * (1 - armorpen) ) ) ) );
				//} else {
			//		finalDamage = finalDamage = modifiedDamage;
			//	}
				currentHealth -= finalDamage;
			}
			// Raise MonsterDamaged event
			if (MonsterDamaged != null)
				MonsterDamaged (this, finalDamage, killer);

			// Kill if health reaches under 0
			if (currentHealth <= 0) {
				KillMonster (source, killer);
				ded = true;
			}

		}
	}

	public void FixedMovespeed(float value, float duration){
		MS.Value = value;
		MS.locked = true;
		MyTimer t = ValueStore.sharedInstance.timerManagerInstance.StartTimer (duration);
		t.TimerElapsed += FixedMoveSpeedEnded;
	}

	public void FixedMoveSpeedEnded(){
		MS.locked = false;
	}

	public void KillMonster(DamageSource source, IAttacker killer)
	{
		if (!ded) {
			// Raise enemydied event
			m.OnEnemyDied (this);

			// Add death particle effects
			GameObject deathParticle = (GameObject)Instantiate (m.deathParticlePrefab, myTransform.position, myTransform.rotation);
			Destroy (deathParticle, 2f);

			ValueStore vs = ValueStore.sharedInstance;
			if (source == DamageSource.Normal) { // Killed by Archers
				vs.waveManagerInstance.totalEnemiesSlain++;
				float value = silverValue * (1 + SaveData.GetUpgrade (UpgradeType.SilverIncrease).CurrentValue);
				vs.Silver += value;
				vs.silverEarned += value;
			} else if (source == DamageSource.Exit) { // Killed by leaving screen
				vs.lives -= livesValue;
			}

			if (vs.lives <= 0 && ValueStore.sharedInstance.active) {
				vs.lives = 0;
				vs.GameOver (GameStatus.Loss);
			} else if (vs.waveManagerInstance.enemiesRemainingThisWave == 0 && vs.waveManagerInstance.curWave == vs.waveManagerInstance.totalWaves &&
			          vs.active && vs.lives > 0) { // End game when all waves end
				vs.GameOver (GameStatus.Win);
			}

			vs.UpdateStats ();
			Destroy (myTransform.root.gameObject);
		}
	}

	public virtual void SetPath(int entrance, int exit){
		List<Path> relevantPaths;
		if (entrance != 0 && exit != 0) {
			relevantPaths = m.paths.Where (x => x.entrance == entrance && x.exit == exit).ToList ();
		} else if (entrance == 0 && exit != 0) {
			relevantPaths = m.paths.Where (x => x.exit == exit).ToList ();
		} else if (entrance != 0 && exit == 0) {
			relevantPaths = m.paths.Where (x => x.entrance == entrance).ToList ();
		}else {
			relevantPaths = m.paths.ToList ();
		}
		int random = Random.Range (1, relevantPaths.Count + 1);
		currentPath = relevantPaths [random - 1];
		startPosition = currentPath.waypoints [currentWaypoint].transform.position;
		endPosition = currentPath.waypoints [currentWaypoint + 1].transform.position;
		pathLength = Mathf.Abs(Vector3.Distance(startPosition, endPosition));
	}

	public void RecheckTargeter(){
		if (targetedBy.Count > 1 && isAboutToDie) {
			Tower t;// t = targetedBy.FirstOrDefault (x => x.bulletsOut.Count > 0 && x.bulletsOut.FirstOrDefault (b => b.target == transform && b.isAboutToKill));
			//if (t == null) {
			//t = targetedBy.FirstOrDefault (x => x.monstersInRange.Count == 1);
			if (targetedBy.All (x => x.monstersInRange.Count > 1)) {
				t = targetedBy.FirstOrDefault (x => ValueStore.sharedInstance.monsterManagerInstance.DoesKill(this, x.AD.Value, x.AP.Value));
			} else {
				t = targetedBy.FirstOrDefault (x => x.monstersInRange.Count == 1 && ValueStore.sharedInstance.monsterManagerInstance.DoesKill(this, x.AD.Value, x.AP.Value));
			}
			//}
			if (t != null) {
				List<Tower> l = targetedBy;
				l.Remove (t);
				foreach (Tower to in l.ToList()) {
					targetedBy.Remove (to);
				//	Debug.Log (t);
					to.RecheckTarget ();
				}
			}
		}
	}
	public void HandleMovement(){
		if (currentWaypoint < currentPath.waypoints.Count -1)
		{
			float speed = MS.Value * Time.deltaTime;
			step = speed / pathLength;
			distanceTravelled += speed;
			progress += step;
		}
		myTransform.root.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp(progress, progress, 1));

		deltaX = Mathf.Abs(endPosition.x - startPosition.x);
		deltaY = Mathf.Abs(endPosition.y - startPosition.y);

		if (directionSwitchTimer.GetCooldownRemaining () <= 0) {
			FixDirection ();
			directionSwitchTimer.ResetTimer (0.4f);
		}
		previousX = myTransform.position.x;
		previousY = myTransform.position.y;

		if (myTransform.root.position == endPosition)
		{
			progress = 0;
			currentWaypoint++;
			FixDirection ();
			startPosition = currentPath.waypoints [currentWaypoint].transform.position;
			endPosition = currentPath.waypoints [currentWaypoint + 1].transform.position;
			pathLength = Mathf.Abs(Vector3.Distance(startPosition, endPosition));
		}

	}

	public void FixDirection(){
		float wdeltaX = endPosition.x - startPosition.x;
		float wdeltaY = endPosition.y - startPosition.y;
		side.transform.rotation = Quaternion.AngleAxis (0, Vector3.up);
		if (endPosition.x >= startPosition.x && (deltaX >= deltaY * 0.8f)) {
			side.SetActive (true);
			down.SetActive (false);
			up.SetActive (false);
		} else if (endPosition.x < startPosition.x && (deltaX > deltaY * 0.8f)) {
			side.SetActive (true);
			down.SetActive (false);
			up.SetActive (false);
			side.transform.rotation = Quaternion.AngleAxis (180, Vector3.up);
		} else if (endPosition.y >= startPosition.y && (deltaY >= deltaX)) {
			up.SetActive (true);
			side.SetActive (false);
			down.SetActive (false);
		} else {
			down.SetActive (true);
			side.SetActive (false);
			up.SetActive (false);
		}
	}
		
	public void OnModifierEnded(Modifier m){
		
	}

	public void AddModifier(Modifier m, StackOperation s, int stackLimit){
		if (GetModifier (m.name) != null) { // if Modifier already exists
			Modifier x = GetModifier (m.name);
			if (s == StackOperation.Additive) { 
				if (stackLimit != 0) {
					// if still didnt reach stack limit
					if (x.currentStack < stackLimit) {
						// inc value and reset duration
						x.value += m.value;
						if (m.lifetime == ModifierLife.Temporary) {
							x.cdTimer.Duration = m.cdTimer.Duration;
						}				
						x.currentStack++;
					} else {
						if (m.lifetime == ModifierLife.Temporary) {
							x.cdTimer.Duration = m.cdTimer.Duration;
						}
					}
				} else if (stackLimit == 0) { // if stacklimit is 0 then it stacks infinitly
					x.value += m.value;
					if (m.lifetime == ModifierLife.Temporary) {
						x.cdTimer.Duration = m.cdTimer.Duration;
					}				
				}
			} else if (s == StackOperation.HighestValue) {
				float mA = m.value < 0 ? -m.value : m.value;
				float xA = x.value < 0 ? -x.value : x.value;

				if (mA > xA) { // if passed modifier is higher value than current
					x.value = m.value;
					if (m.lifetime == ModifierLife.Temporary) {
						x.cdTimer.Duration = m.cdTimer.Duration;
					}
				} else if (mA == xA) {
					if (m.lifetime == ModifierLife.Temporary) {
						x.cdTimer.Duration = m.cdTimer.Duration;
					}
				}
			}
		} else { // add modifier if it doesnt exist
			modifiers.Add (m);
			GetModifier(m.name).currentStack++;
		}
	}

	public void RemoveModifier(Name name){
		foreach (Modifier x in modifiers) {
			if (x.name == name)
				modifiers.Remove (x);
		}
	}

	public Modifier GetModifier(Name name){
		return modifiers.FirstOrDefault (x => x.name == name);
	}

	public IEnumerator ColorFade(List<SpriteRenderer> s)
	{
		for (int i = 0; i < 10; i++)
		{
			foreach (var item in s) {
				item.color = Color.Lerp (item.color, Color.white, 0.1f * (i + 1));
			}
			yield return new WaitForSeconds(0.05f);
		}
	}


	public void ApplyMovementEffect(string type, float value, float duration, string source)
	{
		/*if (type.Equals ("Slow")) {
			if (!speedChangeSource.Contains (source)) {
				GameObject movementEffect = Instantiate (movementEffectPrefab);
				MovementEffect me = movementEffect.GetComponent<MovementEffect> ();
				me.value = value;
				me.duration = duration;
				me.target = this;
				me.source = source;
				movementEffects.Add (movementEffect);
				speedChangeSource.Add (source);
			} else {
				foreach (GameObject item in movementEffects) {
					if (item.GetComponent<MovementEffect> ().source == source) {
						if (value > item.GetComponent<MovementEffect> ().value) {
							item.GetComponent<MovementEffect> ().value = value;
							item.GetComponent<MovementEffect> ().duration = duration;
						}else 
							item.GetComponent<MovementEffect> ().duration = duration;
						return;
					}
				}
			}
		} else if (type.Equals ("Stun")) {
			StartCoroutine (Stun (duration));
		}*/
	}
		
	void OnDestroy(){
		ModifierEnded -= OnModifierEnded;
	}
}

public enum DamageSource
{
	Normal,
	Exit
}

public class MyColor{

	public Color color;
	public int importance;

	public MyColor(Color c, int importance){
		this.color = color;
		this.importance = importance;
	}
}
