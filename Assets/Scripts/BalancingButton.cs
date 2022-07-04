using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BalancingButton : MonoBehaviour {

	public int action;
	Button b;
	Dropdown d;

	void Start(){
		b = GetComponent<Button> ();
		b.onClick.AddListener (OnClick);

		d = FindObjectOfType<Dropdown> ();
	}

	void Update(){

	}

	public void OnClick(){
		if (action == 1) {
			DataService.Instance.SaveData.upgradeList [d.value].level += 1;
		} else if (action == 2) {
			DataService.Instance.SaveData.upgradeList [d.value].level -= 1;
		}
		DataService.Instance.WriteSaveData ();
	}
}
