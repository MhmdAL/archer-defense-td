using UnityEngine;
using System.Collections;
using TMPro;

public class BalancingDpsText : MonoBehaviour {

	public Tower t;
	[HideInInspector]	public TextMeshProUGUI textObject;
	[HideInInspector]	public float damageValue;

	void Awake(){
		textObject = GetComponent<TextMeshProUGUI> ();
		// Monster.UnitDamaged += OnMonsterDamaged;
	}

	public void OnMonsterDamaged(Unit m, float damage, IAttacking source){
		if (source.GetType() == t.GetType() ) {
			damageValue += damage;
			textObject.text = "Damage:  " + damageValue;
		}
	}

	void OnDisable(){
		// Monster.UnitDamaged -= OnMonsterDamaged;
	}
}
