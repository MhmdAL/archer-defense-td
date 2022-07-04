using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class BalancingSpawnDropdown : MonoBehaviour {

	[HideInInspector]	public TMP_Dropdown d;
	public List<GameObject> monsterPrefabs; 

	void Awake () {
		d = GetComponent<TMP_Dropdown> ();
		foreach (var item in monsterPrefabs) {
			d.options.Add(new TMP_Dropdown.OptionData(item.name));
		}

	}
}
