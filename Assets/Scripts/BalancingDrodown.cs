using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class BalancingDrodown : MonoBehaviour {

	Dropdown d;

	void Awake () {
		d = GetComponent<Dropdown> ();
		foreach (var item in DataService.Instance.SaveData.upgradeList) {
			d.options.Add(new Dropdown.OptionData(item.upgradeTitle));
		}

	}

	void FixedUpdate(){
		foreach (var item in d.options.ToList()) {
			ShopUpgrade s = DataService.Instance.SaveData.upgradeList [d.options.IndexOf(item)];
			item.text = s.upgradeTitle + "   " + s.level + "/" + s.maxLevel;
			d.RefreshShownValue ();
		}
	}
}
