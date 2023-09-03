using UnityEngine;
using System.Collections;

public class UtilityArcherSlowCircle : MonoBehaviour {

	[HideInInspector]	public UtilityArcher owner;

	void OnTriggerEnter2D(Collider2D c){
		Monster m = c.GetComponent<Monster> ();
		if (m != null) {
			// Apply stun
			if (m.StunTimer.GetTimeRemaining () <= 0) {
				ShopUpgrade uT4 = SaveData.GetUpgrade (UpgradeType.Utility_4);
				ShopUpgrade aT5 = SaveData.GetUpgrade (UpgradeType.All_1);
				float stunDuration = aT5.level > 0 ? uT4.CurrentValue * (1 + aT5.CurrentValue / 2) : uT4.CurrentValue;

				m.MoveSpeed.Modify(0, BonusOperation.OverallMultiplier, Name.Utility_Stun.ToString(), stunDuration, 1);

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

				m.StunTimer.Restart (stunDuration + 4);
			}

			// Apply Slow
			owner.ApplySlow(m);

			// Apply Vulnerability			
			var val = owner.baseVulnerabilityValue + SaveData.GetUpgrade(UpgradeType.Utility_3).CurrentValue;
			m.DamageModifier.Modify(val, BonusOperation.Percentage, Name.Utility_Vulnerability.ToString(), 2, 1);
		}
	}

	public void DeApplyStunModifier(IModifiable m){
		Monster mon = m as Monster;
		mon.anim.speed = 1;

		mon.stunImage.SetActive (false);
	}
}
