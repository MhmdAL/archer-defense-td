using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class RequiredUpgrade{
	public UpgradeType upgrade;
	public int level;
}

public class ShopButton : MonoBehaviour {

	// Public fields
	public UpgradeType type;

	public GameObject upgradeLock;
	public GameObject upgradeLevelObj;
	public GameObject upgradeMaxed;

	public RequiredUpgrade[] requirementsList;

	public Sprite maxedSprite;

	public Color maxedColor;

	public TextMeshProUGUI levelIndicatorText;

	public Image templateImage;

	[NonSerialized]
	public ShopUpgrade upgrade;

	// Private fields
	private ShopHandler sh;

	private string requirements;


	void Start(){
		sh = ShopHandler.sharedInstance;

		sh.Upgraded += OnUpgraded;

		upgrade = SaveData.GetUpgrade (type);

		UpdateVisuals ();

		SetRequirementsText ();
	}

	public void SetRequirementsText(){
		requirements = "<size=20><color=red>Requires:</size></color>";

		for (int i = 0; i < requirementsList.Length; i++) {
			Color c = Color.red;
			UpgradeType ut = SaveData.GetUpgrade(requirementsList[i].upgrade).ID;
			int lvl = requirementsList[i].level;
			requirements += "\n" + SaveData.GetUpgrade (ut).upgradeTitle + "   <color=green>level " + lvl + "</color>";  
		}
	}

	// Gets called when any upgrade is upgraded
	public void OnUpgraded(ShopUpgrade s){
		UpdateVisuals ();
	}

	public void UpdateVisuals(){
		UpdateLock ();

		levelIndicatorText.text = upgrade.level + "/" + upgrade.maxLevel;

		if (upgrade.level == upgrade.maxLevel) {
			templateImage.sprite = maxedSprite;
			upgradeMaxed.SetActive (true);

			levelIndicatorText.enableVertexGradient = false;
			levelIndicatorText.color = maxedColor;
			levelIndicatorText.outlineColor = (Color32)Color.white;
		}
	}

	public void UpdateLock(){
		if (IsUnlocked ()) {
			upgradeLock.SetActive (false);
			upgradeLevelObj.SetActive (true);
		} else {
			upgradeLevelObj.SetActive (false);
		}
	}

	public bool IsUnlocked(){
		ShopUpgrade s = SaveData.GetUpgrade (type);
		if (requirementsList.Length != 0) {
			//string[] reqs = s.requirements.Split (',');
			for (int i = 0; i < requirementsList.Length; i++) {
				//string[] reqsDetail = reqs [i].Split (':');
				int lvl = requirementsList [i].level;
				if (SaveData.GetUpgrade (requirementsList[i].upgrade).level < lvl) {
					return false;
				}
			}
		}
		return true;
	}

	public void OnClick () {	
		sh.lastClickedShopButton = this;
		if (IsUnlocked ()) {
			sh.OpenUpgradeMenu (this);
		} else {
			sh.OpenRequirementsMenu (this, requirements, upgrade.upgradeTitle);
		}
	}
}
