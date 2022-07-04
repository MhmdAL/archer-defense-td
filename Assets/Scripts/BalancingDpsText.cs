﻿using UnityEngine;
using System.Collections;
using TMPro;

public class BalancingDpsText : MonoBehaviour {

	public Tower t;
	[HideInInspector]	public TextMeshProUGUI textObject;
	[HideInInspector]	public float damageValue;

	void Awake(){
		textObject = GetComponent<TextMeshProUGUI> ();
		Monster.MonsterDamaged += OnMonsterDamaged;
	}

	public void OnMonsterDamaged(Monster m, float damage, IAttacker source){
		if (source.GetType() == t.GetType() ) {
			damageValue += damage;
			textObject.text = "Damage:  " + damageValue;
		}
	}

	void OnDisable(){
		Monster.MonsterDamaged -= OnMonsterDamaged;
	}
}
