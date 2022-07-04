using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public enum BuffIndicatorType
{
	ATTACK_RANGE,
	ATTACK_DAMAGE
}

public enum ArcherType{
	ClassicArcher,
	RapidArcher,
	LongArcher,
	UtilityArcher
}

public class Tower : MonoBehaviour, IPointerClickHandler, IModifiable, IAttacker
{
	#region fields
	public delegate void ModifierEndedEventHandler(Modifier m);
	public event ModifierEndedEventHandler ModifierEnded;

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

	public float UpgradeCost {
		get {
			return baseUpgradeCost + (10 * level);
		}
		set {
			upgradeCost = value;
		}
	}

	public List<Stat> stats;

	public ArcherType archerSpeciality;

	public GameObject upgradeAnimationPrefab;
	public GameObject archerShotParticle;

	public GameObject bullet;
	public GameObject circle;
	public GameObject cooldownBar;

	public SpriteRenderer archerTowerRenderer;

	public BuffIndicatorPanel buffIndicatorPanel;

	public Transform upgradeAnimSpawnPoint;
	public Transform arrowSpawnPoint;

	public Stat AD = new Stat(Type.ATTACK_DAMAGE);
	public Stat AS = new Stat(Type.ATTACK_SPEED);
	public Stat AR = new Stat(Type.ATTACK_RANGE);
	public Stat AP = new Stat(Type.ARMOR_PENETRATION);

	public float combatTimer;
	public float baseUpgradeCost;
	public float bulletSpeed;
	public float bulletRadius;

	public Sprite icon;

	public string title;

	public int maxLevel;

	public TextAsset archerUpgrades;

	[HideInInspector]	
	public GameObject owner;

	[HideInInspector]	
	public bool active;

	[HideInInspector]
	public float fullcooldown;

	[HideInInspector]
	public float silverSpent;
			
	[HideInInspector]
	public int cost;

	[HideInInspector]
	public int level;

	[HideInInspector]	
	public List<Modifier> modifiers = new List<Modifier>();

	[HideInInspector]	
	public List<Monster> monstersInRange;

	protected AudioManager audioManager;

	protected Animator anim;

	protected int consecutiveShots;
	protected int shotNumber;

	private List<Monster> monstersInScene;
	private List<Monster> targets;

	private List<Modifier> towerUpgrades = new List<Modifier>();

	private CooldownTimer ShotCooldown;
	private CooldownTimer CombatCooldown;
	private CooldownTimer DpsCooldown;

	private float upgradeCost;

	private Transform cooldownBarTransform;
	private Transform rangeCircleTransform;

	private Vector3 rangeCircleScale;
	private Vector3 cooldownBarScale;

	private GameObject cooldownBarParent;

	private bool isInCombat = false;

	private List<SpriteRenderer> attachedRenderers;

	#endregion

    void Awake()
    {
		ModifierEnded += OnModifierEnded;

		stats = new List<Stat> (){ AD, AS, AR, AP };

		// Initalize cooldown timers
		InitializeValues();

		// Add modifiers that apply from the start
		AddStartingModifiers ();

		monstersInRange = new List<Monster>();
		targets = new List<Monster>{null, null};

		AdjustSortingOrder ();

		// Set values from file, used when upgrading tower
		SetUpgradeValues ();

		// Adjust stats to apply all modifiers
		AdjustStats ();

		anim = GetComponent<Animator> ();

		audioManager = ValueStore.sharedInstance.audioManagerInstance;
    }

	public virtual void InitializeValues(){
		silverSpent = cost;

		ShotCooldown = new CooldownTimer(0);
		CombatCooldown = new CooldownTimer(2);
		DpsCooldown = new CooldownTimer (15);

		cooldownBarTransform = cooldownBar.transform;
		rangeCircleTransform = circle.transform;

		rangeCircleTransform.position = transform.position;

		cooldownBarScale = cooldownBarTransform.localScale;
		rangeCircleScale = rangeCircleTransform.localScale;

		cooldownBarParent = cooldownBarTransform.parent.gameObject;

		AD.baseValue = SaveData.DEFAULT_AD * (1 + SaveData.GetUpgrade (UpgradeType.AD).CurrentValue);
		AS.baseValue = SaveData.DEFAULT_AS * (1 + SaveData.GetUpgrade (UpgradeType.AS).CurrentValue);
		AR.baseValue = SaveData.DEFAULT_AR * (1 + SaveData.GetUpgrade (UpgradeType.AR).CurrentValue);
		AP.baseValue = SaveData.GetUpgrade (UpgradeType.AP).CurrentValue;
	}

	public virtual void AddStartingModifiers(){
		
	}

    void FixedUpdate()
	{
		// adjust range circle and CD bar
		UpdateTowerVisuals ();

		// shoot when cooldown ends and adjust stats
		if (ShotCooldown.GetCooldownRemaining () <= 0) {
			if (active) {
				ShootNearestMonster ();
				if (targets [0] == null) {
					ShotCooldown.ResetTimer (0.4f);
				}
			} else {
				ShotCooldown.ResetTimer (fullcooldown);
			}
			AdjustStats ();
		}
			
		if (CombatCooldown.GetCooldownRemaining () <= 0) {
			isInCombat = false;
			consecutiveShots = 0;
		}
			
		//Debug.Log (AD.Value);
		//Debug.Log (AS.Value);
		//Debug.Log ("Flat bonus: " + AR.flatBonus + "Bonus: " + AR.BonusValue + "Total: " + AR.Value);

	}

	public virtual void ShootNearestMonster(){
		// Detect monsters in range
		monstersInScene = ValueStore.sharedInstance.monsterManagerInstance.MonstersInScene;
		monstersInRange.Clear();
		foreach (Monster x in monstersInScene) {
			if (IsInRange (x) && !monstersInRange.Contains(x)) {
				monstersInRange.Add (x);
			} else if(!IsInRange (x) && monstersInRange.Contains(x)) {
				monstersInRange.Remove (x);
			}
		}
		// Remove monsters who leave range from list of targets
		foreach (Monster t in targets) {
			if (!IsInRange (t)) {
				if (targets [0] != null && targets [0].targetedBy.Count > 0) {
					if (targets [0].targetedBy.FirstOrDefault (x => x == this))
						targets [0].targetedBy.Remove (this);
				}
				targets [targets.IndexOf (t)] = null;
			}
		}
		// Set targets
		SetTargets();

		if (targets [0] != null) {
			if (ValueStore.sharedInstance.monsterManagerInstance.DoesKill (targets [0], AD.Value, AP.Value)) {
				targets [0].isAboutToDie = true;
			}
		}
		//if (targets [1] == null && monstersInRange.Count > 1 && monstersInRange [0] != targets [0]) {
		//	targets [1] = monstersInRange [0];
		//} else if (targets [1] == null && monstersInRange.Count > 1 && monstersInRange [0] == targets [0]) {
		//	targets [1] = monstersInRange [1];
		//}
		// Shoot targets
		if (targets [0] != null) {
			shotNumber += 1;
			Shoot (targets [0], (GameObject)Instantiate (bullet, arrowSpawnPoint.position, Quaternion.identity), AD.Value, AP.Value, bulletRadius);
		}
	}
	public virtual void Shoot(Monster target, GameObject projectile, float bulletDamage, float armorpen, float radius){
		Vector3 dir = target.transform.position - projectile.transform.position;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
		projectile.transform.rotation = Quaternion.AngleAxis (angle + 90, Vector3.forward);

		Projectile bullet = projectile.GetComponentInChildren<Projectile> ();
		if (target != null) {
			if (ValueStore.sharedInstance.monsterManagerInstance.DoesKill (target, bulletDamage, armorpen)) {
				bullet.isAboutToKill = true;
			}
		}

		//Instantiate (archerShotParticle, arrowSpawnPoint.position, Quaternion.identity);
		bullet.speed = bulletSpeed;
		bullet.ownerTower = this;
		bullet.targetMonster = target;
		bullet.damage = bulletDamage;
		bullet.armorPen = armorpen;
		bullet.radius = radius;
		bullet.shotNumber = shotNumber;
		bullet.currentProjectile = projectile.ToString (); 
		isInCombat = true;
		CombatCooldown.ResetTimer (combatTimer);
		// set cooldown back to full duration
		ShotCooldown.ResetTimer (fullcooldown);
	}

	private void AdjustSortingOrder(){
		// adjust sorting order of the tower components
		attachedRenderers = transform.GetComponentsInChildren<SpriteRenderer>(true).ToList();
		foreach (var item in attachedRenderers) {
			item.sortingOrder += Mathf.RoundToInt ((transform.position.y) * -150);	
		}
	}

	public void SetTargets(){
		if (monstersInRange.Count > 0) { // If there is atleast 1 enemy in range
			monstersInRange.Sort ((x, y) => y.distanceTravelled.CompareTo (x.distanceTravelled)); // Sort enemies in range by how far they travelled

			if (targets [0] == null && monstersInRange [0] != targets [1]) { // If primary and secondary targets are null
				
				if (monstersInRange.Count > 1) { // If there is more than 1 enemy in range
					Monster someTarget = monstersInRange.FirstOrDefault (x => x.isAboutToDie == false); // Try to target enemies that arent about to die
					if (someTarget != null) { 
						targets [0] = someTarget;
					} else { // If non are found then default to the farthest enemy
						targets [0] = monstersInRange [0];
					}
				} else {
					targets [0] = monstersInRange [0];
				}

			} else if (targets [0] == null && monstersInRange [0] == targets [1]) { // If primary target is null and secondary target is not null
				targets [0] = monstersInRange [0]; // Set as primary instead of secondary
				targets [1] = null;
			}
			if (!targets [0].targetedBy.Contains (this)) { 
				targets [0].targetedBy.Add (this);
			}
			targets [0].RecheckTargeter (); // Make sure enemy isnt about to die
		}
	}
		
	public void RecheckTarget(){
		targets [0] = null;
		SetTargets ();
	}

	public void OnModifierEnded(Modifier m){
		
	}

	public void AddModifier(Modifier m, StackOperation s, int stackLimit){
		// TODO: fix multiple If checks
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
				if (Mathf.Abs (m.value) > x.value) { // if passed modifier is higher value than current
					x.value = m.value;
					if (m.lifetime == ModifierLife.Temporary) {
						x.cdTimer.Duration = m.cdTimer.Duration;
					}
				} else if (Mathf.Abs (m.value) == x.value) {
					if (m.lifetime == ModifierLife.Temporary) {
						x.cdTimer.Duration = m.cdTimer.Duration;
					}
				}
			}
		} else { // add modifier if it doesnt exist
			modifiers.Add (m);
			GetModifier(m.name).currentStack++;
		}
		AdjustStats ();
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
		
	/*
	public virtual void AdjustStats(){
		List<Modifier> banList = new List<Modifier>();
		float bonusAD, bonusAS, bonusAR;
		float atkSpdMod = 0;
		float atkDmgMod = 0;
		float atkRngMod = 0;
		float slowValueMod = 0;
		if (modifiers.Count > 0) {
			foreach (Modifier x in modifiers) {
				if (x.active == false)
					banList.Add(x);
				if (x.active == true) {
					if (x.type == Type.ATTACK_SPEED) {
						atkSpdMod += x.value;
					} else if (x.type == Type.ATTACK_DAMAGE) {
						atkDmgMod += x.value;
					} else if (x.type == Type.ATTACK_RANGE) {
						atkRngMod += x.value;
					} else if (x.type == Type.SLOW_ON_ATTACK) {
						slowValueMod += x.value;
					}
				}
			}
		}
		foreach (Modifier m in banList) {
			modifiers.Remove (m);
			ModifierEnded (m);
		}
		baseDamage = SaveData.DEFAULT_AD * (1 + SaveData.GetUpgrade(UpgradeType.AD).CurrentValue);
		baseAttackSpeed = SaveData.DEFAULT_AS * (1 + SaveData.GetUpgrade(UpgradeType.AS).CurrentValue);
		baseRange = SaveData.DEFAULT_AR * (1 + SaveData.GetUpgrade(UpgradeType.AR).CurrentValue);
		bonusAD = baseDamage * (atkDmgMod);
		bonusAS = baseAttackSpeed * (atkSpdMod);
		bonusAR = baseRange * (atkRngMod);
		attackSpeed = baseAttackSpeed + bonusAS;
		damage = baseDamage + bonusAD;
		range = baseRange + bonusAR;

		slowValue = baseSlowValue + (baseSlowValue * slowValueMod);

		armorpen = SaveData.GetUpgrade (UpgradeType.AP).CurrentValue;

		fullcooldown = 1 / attackSpeed;
	}
	*/

	public virtual void AdjustStats(){
		// TODO: Fix this mess of a method..

		foreach (var item in Stats) {
			item.multiplier = 0;
			item.flatBonus = 0;
		}

		if (modifiers.Count > 0) {
			foreach (Modifier m in modifiers.ToList()) {
				if (m.active == false) {
					m.DeApply (this);
					modifiers.Remove (m);
					if(ModifierEnded != null)
						ModifierEnded (m);
				}

				if (m.active == true) {
					Stat s = Stats.FirstOrDefault (x => x.type == m.type);
					for (int i = 0; i < m.currentStack; i++) {
						m.Apply (this);
					}

					if (s != null) {
						if (m.bonusOperation == BonusOperation.Percentage) {
							s.multiplier += m.value;
						} else if (m.bonusOperation == BonusOperation.Flat) {
							s.flatBonus += m.value;
						}
					}
				}
			}
		}

		fullcooldown = 1 / AS.Value;
	}

	public void UpdateTowerVisuals(){
		if (active && monstersInRange.Count > 0) { // If tower is active and monsters nearby, update cooldown bar
			cooldownBarScale = new Vector3 (1, Mathf.Clamp (ShotCooldown.GetCooldownRemaining () / fullcooldown, 0, 1), 1);
		} else {
			cooldownBarScale = new Vector3 (1, 0, 1);
		}
		cooldownBarTransform.localScale = cooldownBarScale;

		// Set the scale of the range circle to the value of attack range
		rangeCircleScale.y = AR.Value / 2;
		rangeCircleScale.x = AR.Value / 2;

		rangeCircleTransform.localScale = rangeCircleScale;

		// Show/Hide range circle and cooldown bar when tower is clicked/unclicked
		if (ValueStore.sharedInstance.lastClickType == ClickType.Tower) {
			if (gameObject == ValueStore.sharedInstance.lastClicked) {
				circle.SetActive (true);
				cooldownBarParent.SetActive (true);
			} else {
				circle.SetActive (false);
				cooldownBarParent.SetActive (false);
			}
		} else {
			circle.SetActive (false);
			cooldownBarParent.SetActive (false);
		}    
	}

	public bool IsInRange(Monster m){
		if (m) {
			Vector3 targetDistance = m.transform.root.position - transform.position;
			if (targetDistance.magnitude < AR.Value) {
				return true;
			} else {
				return false;
			}
		} else
			return false;
	}

	public virtual void BulletHit(Monster target, Projectile p, List<Monster> aoeTargets, int shotNumber)
    {
		if(target != null)
			target.Damage (p.damage, p.armorPen, DamageSource.Normal, this);

		/*if (aoeTargets != null) {
			foreach (Monster x in aoeTargets.AsQueryable().Take(3).ToList()) {
				ValueStore.sharedInstance.monsterManagerInstance.Damage (x, damage / 2, armorpen, "NormalDeath", this);
			}
		}*/
		if (SaveData.GetUpgrade (UpgradeType.Poison_Arrows).level > 0) {
			StartPoison (target, p);
		}
    }

	public virtual void Upgrade(){
		Instantiate(upgradeAnimationPrefab, upgradeAnimSpawnPoint.position, Quaternion.identity);
		if (level == 0) {
			for (int i = 0; i < towerUpgrades.Count; i++) {
				if (towerUpgrades [i].intendedLevel == level + 1) {
					AddModifier (towerUpgrades [i], StackOperation.Additive, 1);
				}
			}
		} else {
			for (int i = 0; i < towerUpgrades.Count; i++) {
				if (towerUpgrades [i].intendedLevel == level + 1) {
					GetModifier (towerUpgrades [i].name).value += towerUpgrades [i].value;
				}
			}
		}
		silverSpent += UpgradeCost;
		AdjustStats ();
	}

	public virtual string NextUpgradeStats(){
		string x = "";
		for (int i = 0; i < towerUpgrades.Count; i++) {
			if (towerUpgrades [i].intendedLevel == level + 1) {
				x += "<#54DFFBFF>+" + Mathf.Abs(towerUpgrades [i].value) * 100 + "%</color> " + GetDisplayName(towerUpgrades[i].type) + "\n"; 
			}
		}
		return x;
	}

	public virtual void SetUpgradeValues(){
		if (archerSpeciality == ArcherType.ClassicArcher)
			return;
		
		string[] splitFile = new string[]{ "\r\n", "\r", "\n" };
		string[] lines = archerUpgrades.text.Split (splitFile, StringSplitOptions.None);
		string[] upgrades = lines [(int) archerSpeciality - 1].Split ('/');

		for (int x = 0; x < maxLevel; x++) {
			string[] upgradeTypes = upgrades [x].Split (',');

			for (int z = 0; z < upgradeTypes.Length; z++) {
				string[] upgradeValues = upgradeTypes [z].Split (';');
				towerUpgrades.Add (new Modifier (float.Parse (upgradeValues [3]), (Name)int.Parse (upgradeValues [1]),
					(Type)int.Parse (upgradeValues [0]), (BonusOperation)int.Parse (upgradeValues [2]), x + 1));
			}
		}
	}

	public string GetDisplayName(Type t){
		switch (t) {
		case Type.ATTACK_DAMAGE:
			return "Attack Damage";
			break;
		case Type.ATTACK_SPEED:
			return "Attack Speed";
			break;
		case Type.ATTACK_RANGE:
			return "Attack Range";
			break;
		case Type.SLOW_ON_ATTACK:
			return "Slow";
			break;
		default:
			return "";
			break;
		}
	}
		
	public void StartPoison(Monster target, Projectile p){
		ShopUpgrade poison = SaveData.GetUpgrade (UpgradeType.Poison_Arrows);
		StartCoroutine (DOT (poison.CurrentValue * p.damage, target));
	}

	public IEnumerator ColorFade()
	{
		for (int i = 0; i < 10; i++)
		{
			archerTowerRenderer.color = Color.Lerp (archerTowerRenderer.color, Color.white, 0.1f * (i + 1));
			yield return new WaitForSeconds(0.05f);
		}
	}

	IEnumerator DOT(float dmg, Monster target)
    {
		int tickCount = 10;

		for (int i = 0; i < tickCount; i++)
        {
			if (target != null) {
				target.Damage (dmg/10, AP.Value, DamageSource.Normal, this);
			} else {
				yield return 0;
			}
            yield return new WaitForSeconds(0.2f);
        }
    }

	public float CurrentValue(UpgradeType upgrade){
		return SaveData.GetUpgrade (upgrade).level * SaveData.GetUpgrade (upgrade).valuePerLevel;
	}

	void OnDestroy(){
		ModifierEnded -= OnModifierEnded;

	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData) {	
		ValueStore vs = ValueStore.sharedInstance;
		vs.lastClickedTower = this;
		vs.OnClick (ClickType.Tower, gameObject);
	}
	#endregion

}
public class CooldownTimer{
	public float startTime;
	public float duration;
	public float currentTime;
	public bool isRunning = false;

	private float elapsed;

	public float Elapsed {
		get { 
			if (isRunning) { // If timer is running return elapsed time before the last stop + elapsed time after starting
				return elapsed + (ValueStore.CurrentTime - startTime);
			} else { // If timer is NOT running return elapsed time before the last stop
				return elapsed;
			}
		}
		set{
			elapsed = value;
		}
	}

	public CooldownTimer(float duration){
		this.duration = duration;

		Start ();
	}

	public void Start(){
		if(!isRunning){ // If timer is NOT running then set it to running and set the start time to current time
			isRunning = true;
			startTime = ValueStore.CurrentTime; 
		}
	}

	public void Stop(){ // If timer is running disable it and adjust elapsed time
		if (isRunning) {
			isRunning = false;
			elapsed += ValueStore.CurrentTime - startTime;
		}
	}

	public void ResetTimer(float duration){
		// Reset Elapsed time to 0
		elapsed = 0;
		// Set duration to new duration
		this.duration = duration;
		// Reset starttime to current time
		startTime = ValueStore.CurrentTime;
	}		
		
	public float GetCooldownRemaining(){
		return duration - Elapsed;
	}
}
