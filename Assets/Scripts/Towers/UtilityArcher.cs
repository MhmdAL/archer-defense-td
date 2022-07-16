using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public class UtilityArcher : Tower {

	public UtilityArcherSlowCircle slowCirclePrefab;

	public Stat Slow = new Stat(Type.SLOW_ON_ATTACK);

	public float slowDuration;
	public float utilityT4Cooldown;
	public float baseVulnerabilityValue;
	public float slowRange;

	public GameObject stunAnimationPrefab;

	private CooldownTimer utilityT4CooldownTimer;


	public override void InitializeValues(){
		base.InitializeValues ();
		utilityT4CooldownTimer = new CooldownTimer (0);

		stats.Add (Slow);
	}

	public void Shoot(Monster target, GameObject projectile, float damage, float armorpen, float radius){
		float bulletRadius = 0;
		if (SaveData.GetUpgrade (UpgradeType.Utility_4).level > 0) {
			if (shotNumber % 5 == 0) {
				bulletRadius = 5;
			} else {
				bulletRadius = radius;
			}
		} else {
			bulletRadius = radius;
		}
	}

	public override void OnTargetHit(Monster target, Projectile p, List<Monster> aoeTargets, int shotNumber)
	{
		base.OnTargetHit (target, p, aoeTargets, shotNumber);
		List<Monster> allTargetsHit = new List<Monster> ();
		if (target != null) {
			// Apply slow
			ApplySlow(target);

			// Apply Vulnerability
			target.AddModifier (new Modifier (baseVulnerabilityValue + SaveData.GetUpgrade(UpgradeType.Utility_3).CurrentValue,
				Name.Utility_Vulnerability, Type.DAMAGE_TAKEN, BonusOperation.Percentage, ValueStore.sharedInstance.timerManagerInstance.StartTimer(2)), StackOperation.HighestValue, 1);
			
			// Apply stun if upgraded
			if (SaveData.GetUpgrade(UpgradeType.Utility_4).level > 0 && aoeTargets != null) {
				allTargetsHit = aoeTargets;
				allTargetsHit.Add (target);
				if (shotNumber % 5 == 0 && utilityT4CooldownTimer.GetCooldownRemaining() <= 0) {
					ApplyStun(target.transform);
					utilityT4CooldownTimer.ResetTimer (utilityT4Cooldown);
				}
				/*if (m.currentHealth >= m.maxHealth / 2) {
					if (m.GetModifier (Name.Stun) == null) {
						m.AddModifier (new Modifier (1, Name.Stun, Type.MOVEMENT_SPEED, 0.5f));
						Debug.Log ("yes");
					} else
						m.GetModifier (Name.Stun).Duration = 0.5f;
				} else {
					if (m.GetModifier (Name.Utility_Slow) == null) {
						m.AddModifier (new Modifier (slowValue, Name.Utility_Slow, Type.MOVEMENT_SPEED, 1f));
					} else if (m.GetModifier (Name.Utility_Slow).value <= slowValue) {
						m.GetModifier (Name.Utility_Slow).Duration = 1f;
						m.GetModifier (Name.Utility_Slow).value = slowValue;
					}
				}*/
			}
		}
	}

	public void ApplySlow(Monster m){
		m.AddModifier(new Modifier (Slow.Value, Name.Utility_Slow, Type.MOVEMENT_SPEED, BonusOperation.Percentage, 
			ValueStore.sharedInstance.timerManagerInstance.StartTimer(slowDuration), DeApplySlowModifier), StackOperation.HighestValue, 1);
		
		foreach (var item in m.GetComponentsInChildren<SpriteRenderer>(true)) {
			if (!m.colorChangeBlockList.Contains (item.gameObject)) {
				item.color = Color.blue;
			}
		}
			/*foreach (var item2 in m.colorChangeBlockList) {
				for (int i = 0; i < item2.transform.childCount; i++) {
					if (item.gameObject != item2 && item.gameObject != item2.transform.GetChild (i).gameObject) {
						item.color = Color.blue;
					}
				}
			}*/
		
	}

	public void DeApplySlowModifier(IModifiable m){
		List<SpriteRenderer> ch = new List<SpriteRenderer> ();
		Monster mon = m as Monster;

		/*foreach (var item in mon.GetComponentsInChildren<SpriteRenderer>(true)) {
			foreach (var item2 in mon.colorChangeBlockList) {
				for (int i = 0; i < item2.transform.childCount; i++) {
					if (item.gameObject != item2 && item.gameObject != item2.transform.GetChild (i).gameObject) {
						ch.Add (item);
					}
				}
			}
		}*/
		foreach (var item in mon.GetComponentsInChildren<SpriteRenderer>(true)) {
			if (!mon.colorChangeBlockList.Contains (item.gameObject)) {
				ch.Add (item);
			}
		}

		mon.StartCoroutine (mon.ColorFade(ch));
	}

	public void ApplyStun(Transform t){
		UtilityArcherSlowCircle c = Instantiate (slowCirclePrefab, t.position, Quaternion.identity) as UtilityArcherSlowCircle;
		c.owner = this;
		StartCoroutine (ExpandCircle (c));
	}

	IEnumerator ExpandCircle(UtilityArcherSlowCircle g){
		for (int i = 0; i < 10; i++) {
			g.transform.localScale += new Vector3 (slowRange / 5, slowRange/ 5, 0);
			yield return new WaitForSeconds (0.01f);
		}
		Destroy (g.gameObject);
	}

	public override void Upgrade ()
	{
		base.Upgrade ();

	}

	public override void AdjustStats ()
	{
		base.AdjustStats ();
	}

	public override void AddStartingModifiers ()
	{
		if (SaveData.GetUpgrade (UpgradeType.Utility_2).level > 0) {
			AddModifier (new Modifier (CurrentValue (UpgradeType.Utility_2), Name.ArcherAD_Upgrade, Type.ATTACK_DAMAGE, BonusOperation.Percentage), StackOperation.Additive, 1);		
		}
	}
		
}
