using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : MonoBehaviour, IAttacking {

	public List<Monster> monstersInSpike = new List<Monster> ();
	public float fullcooldown;
	float cooldown;

    public List<TargetHitEffect> OnHitEffects => throw new System.NotImplementedException();

    void Update(){
		cooldown -= Time.deltaTime;	
		if (monstersInSpike.Count > 0) {
			foreach (Monster m in monstersInSpike) {
				if (m != null) {
					if (cooldown <= 0) {
						m.gameObject.GetComponent<Monster> ();
						m.Damage (Mathf.Clamp(SaveData.GetUpgrade(UpgradeType.ArtilleryCooldown).valuePerLevel
							* SaveData.GetUpgrade(UpgradeType.ArtilleryCooldown).level, 3, 100), 0, DamageSource.Normal, this, null);
					}
					// m.AddModifier (new Modifier (-(Mathf.Clamp (SaveData.GetUpgrade (UpgradeType.ArtilleryArrowCount).valuePerLevel
					// 	* SaveData.GetUpgrade (UpgradeType.ArtilleryArrowCount).level, 0.1f, 0.6f)), Name.Spike_Slow, Type.MOVEMENT_SPEED, BonusType.Percentage,
					// 	ValueStore.Instance.timerManagerInstance.StartTimer(0.1f)), StackOperation.HighestValue, 1);
				}
			}
			if (!(cooldown > 0))
				cooldown = fullcooldown;
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		monstersInSpike.Add (coll.gameObject.GetComponent<Monster> ());
	}

	void OnTriggerExit2D(Collider2D coll){
		monstersInSpike.Remove (coll.gameObject.GetComponent<Monster> ());
	}
}
