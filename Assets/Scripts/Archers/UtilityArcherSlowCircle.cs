using UnityEngine;
using System.Collections;

public class UtilityArcherSlowCircle : MonoBehaviour {

	[HideInInspector]	public UtilityArcher owner;

	void OnTriggerEnter2D(Collider2D c){
		Monster m = c.GetComponent<Monster> ();
		if (m != null) {
			// Apply stun
			if (m.stunnable.GetCooldownRemaining () <= 0) {
				ShopUpgrade uT4 = SaveData.GetUpgrade (UpgradeType.Utility_4);
				ShopUpgrade aT5 = SaveData.GetUpgrade (UpgradeType.All_1);
				float stunDuration = aT5.level > 0 ? uT4.CurrentValue * (1 + aT5.CurrentValue / 2) : uT4.CurrentValue;
				m.AddModifier (new Modifier (0, Name.Utility_Stun,
					Type.MOVEMENT_SPEED, BonusOperation.OverallMultiplier, ValueStore.sharedInstance.timerManagerInstance.StartTimer (stunDuration), DeApplyStunModifier),
					StackOperation.HighestValue, 1);
				m.anim.speed = 0;
				m.stunImage.SetActive (true);

				//GameObject g = (GameObject)Instantiate (a.stunAnimationPrefab, m.transform.position, Quaternion.identity);
				//BoxCollider2D bc = m.GetComponent<BoxCollider2D> ();
				//SpriteRenderer sr = g.GetComponentInChildren<SpriteRenderer> ();
				//g.transform.position = bc.bounds.center;
				//g.transform.localScale = bc.bounds.size * 1.2f;
				//g.transform.position += new Vector3 (0, -3, 0);
			    //sr.sortingOrder = m.GetComponent<SpriteRenderer> ().sortingOrder + 1000;
				//Destroy(g, stunDuration);

				m.stunnable.ResetTimer (stunDuration + 4);
			}

			// Apply Slow
			owner.ApplySlow(m);

			// Apply Vulnerability
			m.AddModifier (new Modifier (owner.baseVulnerabilityValue + SaveData.GetUpgrade(UpgradeType.Utility_3).CurrentValue, Name.Utility_Vulnerability,
				Type.DAMAGE_TAKEN, BonusOperation.Percentage, ValueStore.sharedInstance.timerManagerInstance.StartTimer(2)), StackOperation.HighestValue, 1);
		}
	}

	public void DeApplyStunModifier(IModifiable m){
		Monster mon = m as Monster;
		mon.anim.speed = 1;

		mon.stunImage.SetActive (false);
	}
}
